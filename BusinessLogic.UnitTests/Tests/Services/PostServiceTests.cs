using AutoFixture;
using BusinessLogic.Dtos;
using BusinessLogic.Exceptions;
using BusinessLogic.Services;
using BusinessLogic.UnitTests.Customizations;
using BusinessLogic.UnitTests.Mappers;
using DataAccess.Abstractions;
using DataAccess.Entities;
using DataAccess.Enums;
using DataAccess.Specifications;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace BusinessLogic.UnitTests.Tests.Services;
public class PostServiceTests
{
    private readonly Mock<IRepository<Blog>> _blogRepository = new();
    private readonly Mock<IRepository<Post>> _postRepository = new();
    private readonly Mock<IValidator<CreatePostDto>> _createValidator = new();
    private readonly Mock<IValidator<UpdatePostDto>> _updateValidator = new();
    private readonly Mock<IValidator<PaginationParametersDto>> _paginationValidator = new();
    private readonly Mock<TimeProvider> _timeProvider = new();

    private readonly PostService _postService;

    private readonly Fixture _fixture = new();

    public PostServiceTests()
    {
        _fixture.Customize(new DomainCustomization());

        _postService = new(_blogRepository.Object,
            _postRepository.Object,
            TestMapper.Mapper,
            _createValidator.Object,
            _updateValidator.Object,
            _paginationValidator.Object,
            _timeProvider.Object);
    }

    [Fact]
    public async Task Create_ThrowsEntityValidationFailedException_When_DtoIsInvalid()
    {
        var userId = _fixture.Create<Guid>();
        var blogId = _fixture.Create<Guid>();
        var dto = _fixture.Create<CreatePostDto>();
        var validationResult = _fixture.Create<ValidationResult>();
        _createValidator.Setup(v => v.Validate(dto))
            .Returns(validationResult);
        var expectedException = new EntityValidationFailedException(validationResult.Errors);

        var act = () => _postService.Create(userId, blogId, dto);

        await act.Should().ThrowAsync<EntityValidationFailedException>()
            .WithMessage(expectedException.Message);
    }

    [Fact]
    public async Task Create_ThrowsEntityNotFoundException_When_BlogWasNotFound()
    {
        var userId = _fixture.Create<Guid>();
        var blogId = _fixture.Create<Guid>();
        var dto = _fixture.Create<CreatePostDto>();
        _createValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _blogRepository.Setup(b => b.FirstOrDefaultAsync(It.IsAny<BlogByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Blog);

        var act = () => _postService.Create(userId, blogId, dto);

        await act.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage($"Blog with id {blogId} was not found");
    }

    [Fact]
    public async Task Create_ThrowsEntityValidationFailedException_When_UserIsNotOwnerOfBlog()
    {
        var userId = _fixture.Create<Guid>();
        var blog = _fixture.Create<Blog>();
        var dto = _fixture.Create<CreatePostDto>();
        _createValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _blogRepository.Setup(b => b.FirstOrDefaultAsync(It.IsAny<BlogByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(blog);

        var act = () => _postService.Create(userId, blog.Id, dto);

        await act.Should().ThrowAsync<EntityValidationFailedException>()
            .WithMessage("This blog belongs to another user");
    }

    [Fact]
    public async Task Create_SavesPost_When_Success()
    {
        var blog = _fixture.Create<Blog>();
        var userId = blog.UserId;
        var dto = _fixture.Create<CreatePostDto>();
        var expectedCreationDate = _fixture.Create<DateTimeOffset>();
        _createValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _blogRepository.Setup(b => b.FirstOrDefaultAsync(It.IsAny<BlogByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(blog);
        _timeProvider.Setup(t => t.GetUtcNow())
            .Returns(expectedCreationDate);

        _ = await _postService.Create(userId, blog.Id, dto);

        _postRepository.Verify(
            r => r.AddAsync(It.Is<Post>(p => 
                    p.Header == dto.Header
                    && p.CreatedAt == expectedCreationDate 
                    && p.PostItems.Count == dto.PostItems.Count
                    && p.PostItems.Select(i => i.Order).Order().SequenceEqual(Enumerable.Range(0, dto.PostItems.Count))), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Create_ReturnsDto_When_Success()
    {
        var blog = _fixture.Create<Blog>();
        var userId = blog.UserId;
        var dto = _fixture.Create<CreatePostDto>();
        var expectedCreationDate = _fixture.Create<DateTimeOffset>();
        _createValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _blogRepository.Setup(b => b.FirstOrDefaultAsync(It.IsAny<BlogByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(blog);
        _timeProvider.Setup(t => t.GetUtcNow())
            .Returns(expectedCreationDate);

        var result = await _postService.Create(userId, blog.Id, dto);

        result.Should().BeEquivalentTo(
            dto, 
            opt => opt.ExcludingMissingMembers()
                .For(d => d.PostItems)
                .Exclude(i => i.Id)
                .WithStrictOrdering());
        result.CreatedAt.Should().Be(expectedCreationDate);
    }


    [Fact]
    public async Task Update_ThrowsEntityValidationFailedException_When_DtoIsInvalid()
    {
        var userId = _fixture.Create<Guid>();
        var post = _fixture.Create<Post>();
        var dto = _fixture.Create<UpdatePostDto>();
        var validationResult = _fixture.Create<ValidationResult>();
        _updateValidator.Setup(v => v.Validate(dto))
            .Returns(validationResult);
        var expectedException = new EntityValidationFailedException(validationResult.Errors);

        var act = () => _postService.Update(userId, post.Id, dto);

        await act.Should().ThrowAsync<EntityValidationFailedException>()
            .WithMessage(expectedException.Message);
    }

    [Fact]
    public async Task Update_ThrowsEntityNotFoundException_When_PostWasNotFound()
    {
        var userId = _fixture.Create<Guid>();
        var post = _fixture.Create<Post>();
        var dto = _fixture.Create<UpdatePostDto>();
        _updateValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _postRepository.Setup(b => b.FirstOrDefaultAsync(It.IsAny<PostByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Post);

        var act = () => _postService.Update(userId, post.Id, dto);

        await act.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage($"Post with id {post.Id} was not found");
    }

    [Fact]
    public async Task Update_ThrowsEntityValidationFailedException_When_UserIsNotOwnerOfBlog()
    {
        var userId = _fixture.Create<Guid>();
        var post = _fixture.Create<Post>();
        var dto = _fixture.Create<UpdatePostDto>();
        _updateValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _postRepository.Setup(b => b.FirstOrDefaultAsync(It.IsAny<PostByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(post);

        var act = () => _postService.Update(userId, post.Id, dto);

        await act.Should().ThrowAsync<EntityValidationFailedException>()
            .WithMessage("This blog belongs to another user");
    }

    [Fact]
    public async Task Update_UpdatesPostDetails_When_Success()
    {
        var post = _fixture.Build<Post>()
            .Without(p => p.PostItems)
            .Create();
        var userId = post.Blog.UserId;
        var dto = _fixture.Build<UpdatePostDto>()
            .Without(p => p.PostItems)
            .Create();
        _updateValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _postRepository.Setup(b => b.FirstOrDefaultAsync(It.IsAny<PostByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(post);

        var result = await _postService.Update(userId, post.Id, dto);

        _postRepository.Verify(
            r => r.UpdateAsync(
                It.Is<Post>(p => p.IsModidied && p.Header == dto.Header && p.Subheader == dto.Subheader), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
        result.IsModidied.Should().BeTrue();
        result.Header.Should().Be(dto.Header);
        result.Subheader.Should().Be(dto.Subheader);
    }

    [Fact]
    public async Task Update_RemovesOldPostItems_When_Success()
    {
        var post = _fixture.Create<Post>();
        var userId = post.Blog.UserId;
        var dto = _fixture.Build<UpdatePostDto>()
            .Without(p => p.PostItems)
            .Create();
        _updateValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _postRepository.Setup(b => b.FirstOrDefaultAsync(It.IsAny<PostByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(post);

        var result = await _postService.Update(userId, post.Id, dto);

        _postRepository.Verify(
            r => r.UpdateAsync(
                It.Is<Post>(p => p.PostItems.Count == 0),
                It.IsAny<CancellationToken>()),
            Times.Once);
        result.PostItems.Should().BeEmpty();
    }

    [Fact]
    public async Task Update_UpdatesExistingPostItems_When_Success()
    {
        var post = _fixture.Create<Post>();
        post.PostItems = post.PostItems.Select((p, i) => { p.Order = i; return p; }).ToList();
        var userId = post.Blog.UserId;
        var dto = _fixture.Build<UpdatePostDto>()
            .With(p => p.PostItems, post.PostItems
                .Select<PostItem, PostItemDto>(i => i is TextPostItem 
                    ? new TextPostItemDto { Id = i.Id, Text = _fixture.Create<string>(), TextPostType = _fixture.Create<TextPostType>()}
                    : new ImagePostItemDto { Id = i.Id, Alt = _fixture.Create<string>(), Description = _fixture.Create<string>(), LocalFileName = _fixture.Create<string>()})
                .ToList())
            .Create();
        _updateValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _postRepository.Setup(b => b.FirstOrDefaultAsync(It.IsAny<PostByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(post);

        var result = await _postService.Update(userId, post.Id, dto);

        _postRepository.Verify(
            r => r.UpdateAsync(
                It.Is<Post>(p => p.PostItems.Count == dto.PostItems.Count && p.PostItems.Count == post.PostItems.Count),
                It.IsAny<CancellationToken>()),
            Times.Once);
        result.PostItems.Should().BeEquivalentTo(dto.PostItems, opt => opt.ExcludingMissingMembers().WithStrictOrdering());
    }

    [Fact]
    public async Task Update_AddsNewPostItems_When_Success()
    {
        var post = _fixture.Build<Post>()
            .Without(p => p.PostItems)
            .Create();
        var userId = post.Blog.UserId;
        var dto = _fixture.Create<UpdatePostDto>();
        foreach (var item in dto.PostItems)
        {
            item.Id = null;
        }
        _updateValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _postRepository.Setup(b => b.FirstOrDefaultAsync(It.IsAny<PostByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(post);

        var result = await _postService.Update(userId, post.Id, dto);

        _postRepository.Verify(
            r => r.UpdateAsync(
                It.Is<Post>(p => p.PostItems.Count == dto.PostItems.Count),
                It.IsAny<CancellationToken>()),
            Times.Once);
        result.PostItems.Where(p => p is TextPostItemDto)
            .Should()
            .BeEquivalentTo(
                post.PostItems.Where(p => p is TextPostItem), 
                opt => opt.ExcludingMissingMembers().Excluding(i => i.Id).WithStrictOrdering());
        result.PostItems.Where(p => p is ImagePostItemDto)
            .Should()
            .BeEquivalentTo(
                post.PostItems.Where(p => p is ImagePostItem),
                opt => opt.ExcludingMissingMembers().Excluding(i => i.Id).WithStrictOrdering());
    }

    [Fact]
    public async Task Delete_ThrowsEntityNotFoundException_When_PostWasNotFound()
    {
        var userId = _fixture.Create<Guid>();
        var postId = _fixture.Create<Guid>();
        _postRepository.Setup(b => b.FirstOrDefaultAsync(It.IsAny<PostByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Post);

        var act = () => _postService.Delete(userId, postId);

        await act.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage($"Post with id {postId} was not found");
    }

    [Fact]
    public async Task Delete_ThrowsEntityValidationFailedException_When_UserIsNotOwnerOfBlog()
    {
        var userId = _fixture.Create<Guid>();
        var post = _fixture.Create<Post>();
        _postRepository.Setup(b => b.FirstOrDefaultAsync(It.IsAny<PostByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(post);

        var act = () => _postService.Delete(userId, post.Id);

        await act.Should().ThrowAsync<EntityValidationFailedException>()
            .WithMessage("This blog belongs to another user");
    }

    [Fact]
    public async Task Delete_DeletesPost_When_Success()
    {
        var post = _fixture.Create<Post>();
        var userId = post.Blog.UserId;
        _postRepository.Setup(b => b.FirstOrDefaultAsync(It.IsAny<PostByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(post);

        await _postService.Delete(userId, post.Id);

        _postRepository.Verify(p => p.DeleteAsync(post, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetById_ThrowsEntityNotFoundException_When_PostWasNotFound()
    {
        var postId = _fixture.Create<Guid>();
        _postRepository.Setup(b => b.FirstOrDefaultAsync(It.IsAny<PostByIdNoTrackingSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Post);

        var act = () => _postService.GetById(postId);

        await act.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage($"Post with id {postId} was not found.");
    }

    [Fact]
    public async Task GetById_ReturnsDto_When_Success()
    {
        var post = _fixture.Create<Post>();
        _postRepository.Setup(b => b.FirstOrDefaultAsync(It.IsAny<PostByIdNoTrackingSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(post);

        var result = await _postService.GetById(post.Id);

        result.Should().BeEquivalentTo(post, opt => opt.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GetPagedByBlogId_ThrowsEntityValidationException_When_PaginationDtoIsInvalid()
    {
        var blogId = _fixture.Create<Guid>();
        var dto = _fixture.Create<PaginationParametersDto>();
        var validationResult = _fixture.Create<ValidationResult>();
        _paginationValidator.Setup(v => v.Validate(dto))
            .Returns(validationResult);
        var expectedException = new EntityValidationFailedException(validationResult.Errors);

        var act = () => _postService.GetPagedByBlogId(blogId, dto);

        await act.Should().ThrowAsync<EntityValidationFailedException>()
            .WithMessage(expectedException.Message);
    }

    [Fact]
    public async Task GetPagedByBlogId_ReturnsPosts_When_Success()
    {
        var blogId = _fixture.Create<Guid>();
        var posts = _fixture.CreateMany<Post>().ToList();
        var dto = _fixture.Create<PaginationParametersDto>();
        _paginationValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _postRepository.Setup(r => r.ListAsync(It.IsAny<PagedPostsByBlogIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(posts);


        var result = await _postService.GetPagedByBlogId(blogId, dto);

        result.PaginationParameters.Should().Be(dto);
        result.Data.Should().BeEquivalentTo(posts, opt => opt.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GetPaged_ThrowsEntityValidationException_When_PaginationDtoIsInvalid()
    {
        var dto = _fixture.Create<PaginationParametersDto>();
        var validationResult = _fixture.Create<ValidationResult>();
        _paginationValidator.Setup(v => v.Validate(dto))
            .Returns(validationResult);
        var expectedException = new EntityValidationFailedException(validationResult.Errors);

        var act = () => _postService.GetPaged(dto);

        await act.Should().ThrowAsync<EntityValidationFailedException>()
            .WithMessage(expectedException.Message);
    }

    [Fact]
    public async Task GetPaged_ReturnsPosts_When_Success()
    {
        var posts = _fixture.CreateMany<Post>().ToList();
        var dto = _fixture.Create<PaginationParametersDto>();
        _paginationValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _postRepository.Setup(r => r.ListAsync(It.IsAny<PagedPostsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(posts);

        var result = await _postService.GetPaged(dto);

        result.PaginationParameters.Should().Be(dto);
        result.Data.Should().BeEquivalentTo(posts, opt => opt.ExcludingMissingMembers());
    }
}
