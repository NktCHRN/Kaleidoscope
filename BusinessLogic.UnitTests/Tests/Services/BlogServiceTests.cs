using AutoFixture;
using BusinessLogic.Constants;
using BusinessLogic.Dtos;
using BusinessLogic.Exceptions;
using BusinessLogic.Options;
using BusinessLogic.Services;
using BusinessLogic.UnitTests.Customizations;
using BusinessLogic.UnitTests.Mappers;
using DataAccess.Abstractions;
using DataAccess.Entities;
using DataAccess.Specifications;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace BusinessLogic.UnitTests.Tests.Services;
public class BlogServiceTests
{

    private readonly Mock<IValidator<CreateBlogDto>> _createValidator = new();
    private readonly Mock<IValidator<UpdateBlogDto>> _updateValidator = new();
    private readonly Mock<IRepository<Blog>> _blogRepository = new();
    private readonly Mock<IRepository<User>> _userRepository = new();
    private readonly Mock<UserManager<User>> _userManager;
    private readonly Mock<IBlobRepository> _blobRepository = new();
    private readonly Mock<TimeProvider> _timeProvider = new();

    private readonly BlogService _blogService;

    private readonly Fixture _fixture = new();

    public BlogServiceTests() 
    {
        _fixture.Customize(new DomainCustomization());

        _userManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null!, null!, null!, null!, null!, null!, null!, null!);
        var tokenProvidersOptionsMock = new Mock<IOptions<TokenProvidersOptions>>();
        tokenProvidersOptionsMock.Setup(o => o.Value)
            .Returns(_fixture.Create<TokenProvidersOptions>());

        _blogService = new(
            _createValidator.Object,
            _updateValidator.Object,
            _blogRepository.Object,
            _userManager.Object,
            TestMapper.Mapper,
            _userRepository.Object,
            _blobRepository.Object,
            _timeProvider.Object);
    }

    [Fact]
    public async Task Create_ThrowsEntityValidationFailedException_When_DtoIsInvalid()
    {
        var userId = _fixture.Create<Guid>();
        var dto = _fixture.Create<CreateBlogDto>();
        var validationResult = _fixture.Create<ValidationResult>();
        _createValidator.Setup(v => v.Validate(dto))
            .Returns(validationResult);
        var expectedException = new EntityValidationFailedException(validationResult.Errors);

        var act = () => _blogService.Create(userId, dto);

        await act.Should().ThrowAsync<EntityValidationFailedException>()
            .WithMessage(expectedException.Message);
    }

    [Fact]
    public async Task Create_ThrowsEntityNotFoundException_When_UserWasNotFound()
    {
        var userId = _fixture.Create<Guid>();
        var dto = _fixture.Create<CreateBlogDto>();
        _createValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _userRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<UserByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as User);

        var act = () => _blogService.Create(userId, dto);

        await act.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage($"User with id {userId} was not found.");
    }

    [Fact]
    public async Task Create_ThrowsEntityAlreadyExistsException_When_UserAlreadyHasABlog()
    {
        var user = _fixture.Create<User>();
        var dto = _fixture.Create<CreateBlogDto>();
        _createValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _userRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<UserByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var act = () => _blogService.Create(user.Id, dto);

        await act.Should().ThrowAsync<EntityAlreadyExistsException>()
            .WithMessage("User already has a blog.");
    }

    [Fact]
    public async Task Create_ThrowsEntityAlreadyExistsException_When_BlogWithPassedTagAlreadyExists()
    {
        var user = _fixture.Create<User>();
        user.Blog = null;
        var dto = _fixture.Create<CreateBlogDto>();
        _createValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _userRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<UserByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _blogRepository.Setup(r => r.AnyAsync(It.IsAny<BlogByTagSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = () => _blogService.Create(user.Id, dto);

        await act.Should().ThrowAsync<EntityAlreadyExistsException>()
            .WithMessage($"Blog with tag {dto.Tag} already exists.");
    }

    [Fact]
    public async Task Create_SavesBlogAndAddsUserToAuthorRole_When_Success()
    {
        var user = _fixture.Create<User>();
        user.Blog = null;
        var dto = _fixture.Create<CreateBlogDto>();
        _createValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _userRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<UserByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _blogRepository.Setup(r => r.AnyAsync(It.IsAny<BlogByTagSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _ = await _blogService.Create(user.Id, dto);

        _userManager.Verify(m => m.AddToRoleAsync(user, RolesConstants.Author), Times.Once);
    }

    [Fact]
    public async Task Create_ReturnsDto_When_Success()
    {
        var user = _fixture.Create<User>();
        user.Blog = null;
        var dto = _fixture.Create<CreateBlogDto>();
        var expectedDtoTag = dto.Tag.ToLower();
        dto.Tag = $" {dto.Tag} ";     // Check tag trimming & ToLower.
        _createValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _userRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<UserByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _blogRepository.Setup(r => r.AnyAsync(It.IsAny<BlogByTagSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _blogService.Create(user.Id, dto);

        result.Tag.Should().Be(expectedDtoTag);
        result.Description.Should().Be(dto.Description);
    }

    [Fact]
    public async Task Update_ThrowsEntityValidationFailedException_When_DtoIsInvalid()
    {
        var userId = _fixture.Create<Guid>();
        var blogId = _fixture.Create<Guid>();
        var dto = _fixture.Create<UpdateBlogDto>();
        var validationResult = _fixture.Create<ValidationResult>();
        _updateValidator.Setup(v => v.Validate(dto))
            .Returns(validationResult);
        var expectedException = new EntityValidationFailedException(validationResult.Errors);

        var act = () => _blogService.Update(userId, blogId, dto);

        await act.Should().ThrowAsync<EntityValidationFailedException>()
            .WithMessage(expectedException.Message);
    }

    [Fact]
    public async Task Update_ThrowsEntityNotFoundException_When_UserWasNotFound()
    {
        var userId = _fixture.Create<Guid>();
        var blogId = _fixture.Create<Guid>();
        var dto = _fixture.Create<UpdateBlogDto>();
        _updateValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _blogRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<BlogByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Blog);

        var act = () => _blogService.Update(userId, blogId, dto);

        await act.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage($"Blog with id {blogId} was not found.");
    }

    [Fact]
    public async Task Update_ThrowsEntityValidationFailedException_When_UserIsNotOwnerOfTheBlog()
    {
        var userId = _fixture.Create<Guid>();
        var blog = _fixture.Create<Blog>();
        var dto = _fixture.Create<UpdateBlogDto>();
        _updateValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _blogRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<BlogByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(blog);

        var act = () => _blogService.Update(userId, blog.Id, dto);

        await act.Should().ThrowAsync<EntityValidationFailedException>()
            .WithMessage("You cannot update this blog.");
    }

    [Fact]
    public async Task Update_ThrowsEntityAlreadyExistsException_When_BlogWithPassedTagAlreadyExists()
    {
        var userId = _fixture.Create<Guid>();
        var blog = _fixture.Build<Blog>()
            .With(b => b.UserId, userId)
            .Create();
        var dto = _fixture.Create<UpdateBlogDto>();
        _updateValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _blogRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<BlogByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(blog);
        _blogRepository.Setup(r => r.AnyAsync(It.IsAny<BlogByTagSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = () => _blogService.Update(userId, blog.Id, dto);

        await act.Should().ThrowAsync<EntityAlreadyExistsException>()
            .WithMessage($"Blog with tag {dto.Tag} already exists.");
    }

    [Fact]
    public async Task Update_ThrowsValidationFailedException_When_AvatarWasNotFound()
    {
        var userId = _fixture.Create<Guid>();
        var blog = _fixture.Build<Blog>()
            .With(b => b.UserId, userId)
            .Create();
        var dto = _fixture.Create<UpdateBlogDto>();
        _updateValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _blogRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<BlogByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(blog);
        _blogRepository.Setup(r => r.AnyAsync(It.IsAny<BlogByTagSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _blobRepository.Setup(b => b.ExistsAsync(dto.AvatarLocalFileName!))
            .ReturnsAsync(false);

        var act = () => _blogService.Update(userId, blog.Id, dto);

        await act.Should().ThrowAsync<EntityValidationFailedException>()
            .WithMessage($"Image with name {dto.AvatarLocalFileName} was not found");
    }

    [Fact]
    public async Task Update_SavesBlogAndUser_When_Success()
    {
        var userId = _fixture.Create<Guid>();
        var blog = _fixture.Build<Blog>()
            .With(b => b.UserId, userId)
            .Create();
        var dto = _fixture.Create<UpdateBlogDto>();
        var expectedDtoTag = dto.Tag.ToLower();
        dto.Tag = $" {dto.Tag} ";     // Check tag trimming & ToLower.
        _updateValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _blogRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<BlogByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(blog);
        _blogRepository.Setup(r => r.AnyAsync(It.IsAny<BlogByTagSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _blobRepository.Setup(b => b.ExistsAsync(dto.AvatarLocalFileName!))
            .ReturnsAsync(true);

        _ = await _blogService.Update(userId, blog.Id, dto);

        _blogRepository.Verify(
            r => r.UpdateAsync(It.Is<Blog>(b => b.Name == dto.Name
                && b.User.Name == dto.Name
                && b.AvatarLocalFileName == dto.AvatarLocalFileName
                && b.User.AvatarLocalFileName == dto.AvatarLocalFileName
                && b.Tag == expectedDtoTag
                && b.Description == dto.Description), It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Update_ReturnsDto_When_Success()
    {
        var userId = _fixture.Create<Guid>();
        var blog = _fixture.Build<Blog>()
            .With(b => b.UserId, userId)
            .Create();
        var dto = _fixture.Create<UpdateBlogDto>();
        var expectedDtoTag = dto.Tag.ToLower();
        dto.Tag = $" {dto.Tag} ";     // Check tag trimming & ToLower.
        _updateValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _blogRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<BlogByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(blog);
        _blogRepository.Setup(r => r.AnyAsync(It.IsAny<BlogByTagSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _blobRepository.Setup(b => b.ExistsAsync(dto.AvatarLocalFileName!))
            .ReturnsAsync(true);

        var result = await _blogService.Update(userId, blog.Id, dto);

        dto.Tag = expectedDtoTag;
        result.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task GetByTag_ThrowsEntityNotFoundException_When_BlogWasNotFound()
    {
        var tag = _fixture.Create<string>();
        var expectedTag = tag.ToLower();
        _blogRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<BlogByTagSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Blog);

        var act = () => _blogService.GetByTag(tag);

        await act.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage($"Blog with tag {expectedTag} was not found.");
    }

    [Fact]
    public async Task GetByTag_ReturnsDto_When_Success()
    {
        var tag = _fixture.Create<string>();
        var blog = _fixture.Create<Blog>();
        _blogRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<BlogByTagSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(blog);

        var result = await _blogService.GetByTag(tag);

        result.Should().BeEquivalentTo(blog, opt => opt.ExcludingMissingMembers());
    }
}
