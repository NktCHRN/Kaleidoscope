using AutoFixture;
using BusinessLogic.Abstractions;
using BusinessLogic.Dtos;
using BusinessLogic.Exceptions;
using BusinessLogic.Services;
using BusinessLogic.UnitTests.Customizations;
using BusinessLogic.UnitTests.Mappers;
using DataAccess.Abstractions;
using DataAccess.Entities;
using DataAccess.Specifications;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace BusinessLogic.UnitTests.Tests.Services;
public class CommentServiceTests
{
    private readonly Mock<IRepository<Comment>> _commentRepository = new();
    private readonly Mock<IValidator<CreateCommentDto>> _createValidator = new();
    private readonly Mock<IValidator<UpdateCommentDto>> _updateValidator = new();
    private readonly Mock<IValidator<PaginationParametersDto>> _paginationValidator = new();
    private readonly Mock<IRepository<Post>> _postRepository = new();
    private readonly Mock<IRepository<User>> _userRepository = new();
    private readonly Mock<TimeProvider> _timeProvider = new();
    private readonly CommentService _commentService;

    private readonly Fixture _fixture = new();

    public CommentServiceTests() 
    {
        _fixture.Customize(new DomainCustomization());

        _commentService = new(
            _commentRepository.Object,
            _createValidator.Object,
            _updateValidator.Object,
            _paginationValidator.Object,
            TestMapper.Mapper,
            _postRepository.Object,
            _userRepository.Object,
            _timeProvider.Object);
    }

    [Fact]
    public async Task Create_ThrowsEntityValidationFailedException_When_DtoIsInvalid()
    {
        var userId = _fixture.Create<Guid>();
        var postId = _fixture.Create<Guid>();
        var dto = _fixture.Create<CreateCommentDto>();
        var validationResult = _fixture.Create<ValidationResult>();
        _createValidator.Setup(v => v.Validate(dto))
            .Returns(validationResult);
        var expectedException = new EntityValidationFailedException(validationResult.Errors);

        var act = () => _commentService.Create(userId, postId, dto);

        await act.Should().ThrowAsync<EntityValidationFailedException>()
            .WithMessage(expectedException.Message);
    }

    [Fact]
    public async Task Create_ThrowsEntityNotFoundException_When_PostWasNotFound()
    {
        var userId = _fixture.Create<Guid>();
        var postId = _fixture.Create<Guid>();
        var dto = _fixture.Create<CreateCommentDto>();
        _createValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _postRepository.Setup(r => r.GetByIdAsync(postId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Post);

        var act = () => _commentService.Create(userId, postId, dto);

        await act.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage($"Post with id {postId} was not found");
    }

    [Fact]
    public async Task Create_ThrowsEntityNotFoundException_When_UserWasNotFound()
    {
        var userId = _fixture.Create<Guid>();
        var post = _fixture.Create<Post>();
        var dto = _fixture.Create<CreateCommentDto>();
        _createValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _postRepository.Setup(r => r.GetByIdAsync(post.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(post);
        _userRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<UserByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as User);

        var act = () => _commentService.Create(userId, post.Id, dto);

        await act.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage($"User with id {userId} was not found");
    }

    [Fact]
    public async Task Create_SavesComment_When_Success()
    {
        var user = _fixture.Create<User>();
        var post = _fixture.Create<Post>();
        var dto = _fixture.Create<CreateCommentDto>();
        _createValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _postRepository.Setup(r => r.GetByIdAsync(post.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(post);
        _userRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<UserByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _ = await _commentService.Create(user.Id, post.Id, dto);

        _commentRepository.Verify(
            r => r.AddAsync(
                It.Is<Comment>(c => c.Text == dto.Text && c.User == user && c.Post == post), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Create_ReturnsDto_When_Success()
    {
        var user = _fixture.Create<User>();
        var post = _fixture.Create<Post>();
        var dto = _fixture.Create<CreateCommentDto>();
        _createValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _postRepository.Setup(r => r.GetByIdAsync(post.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(post);
        _userRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<UserByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _commentService.Create(user.Id, post.Id, dto);

        result.Text.Should().Be(dto.Text);
        result.User.Should().BeEquivalentTo(user, opt => opt.ExcludingMissingMembers());
        result.UserBlogTag.Should().Be(user.Blog!.Tag);
    }

    [Fact]
    public async Task Update_ThrowsEntityValidationFailedException_When_DtoIsInvalid()
    {
        var userId = _fixture.Create<Guid>();
        var commentid = _fixture.Create<Guid>();
        var dto = _fixture.Create<UpdateCommentDto>();
        var validationResult = _fixture.Create<ValidationResult>();
        _updateValidator.Setup(v => v.Validate(dto))
            .Returns(validationResult);
        var expectedException = new EntityValidationFailedException(validationResult.Errors);

        var act = () => _commentService.Update(userId, commentid, dto);

        await act.Should().ThrowAsync<EntityValidationFailedException>()
            .WithMessage(expectedException.Message);
    }

    [Fact]
    public async Task Update_ThrowsEntityNotFoundException_When_CommentWasNotFound()
    {
        var userId = _fixture.Create<Guid>();
        var commentId = _fixture.Create<Guid>();
        var dto = _fixture.Create<UpdateCommentDto>();
        _updateValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _commentRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<CommentByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Comment);

        var act = () => _commentService.Update(userId, commentId, dto);

        await act.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage($"Comment with id {commentId} was not found");
    }

    [Fact]
    public async Task Update_ThrowsEntityValidationException_When_UserIsNotOwnerOfComment()
    {
        var userId = _fixture.Create<Guid>();
        var comment = _fixture.Create<Comment>();
        var dto = _fixture.Create<UpdateCommentDto>();
        _updateValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _commentRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<CommentByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);

        var act = () => _commentService.Update(userId, comment.Id, dto);

        await act.Should().ThrowAsync<EntityValidationFailedException>()
            .WithMessage("This comment belongs to another user");
    }

    [Fact]
    public async Task Update_SavesComment_When_Success()
    {
        var userId = _fixture.Create<Guid>();
        var comment = _fixture.Create<Comment>();
        comment.UserId = userId;
        var dto = _fixture.Create<UpdateCommentDto>();
        _updateValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _commentRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<CommentByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);

        _ = await _commentService.Update(userId, comment.Id, dto);

        _commentRepository.Verify(
            r => r.UpdateAsync(
                It.Is<Comment>(c => c.Text == dto.Text && c.IsModidied),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Update_ReturnsDto_When_Success()
    {
        var userId = _fixture.Create<Guid>();
        var comment = _fixture.Create<Comment>();
        comment.UserId = userId;
        var dto = _fixture.Create<UpdateCommentDto>();
        _updateValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _commentRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<CommentByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);

        var result = await _commentService.Update(userId, comment.Id, dto);

        result.Text.Should().Be(dto.Text);
        result.IsModidied.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_ThrowsEntityNotFoundException_When_CommentWasNotFound()
    {
        var userId = _fixture.Create<Guid>();
        var commentId = _fixture.Create<Guid>();
        _commentRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<CommentByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Comment);

        var act = () => _commentService.Delete(userId, commentId);

        await act.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage($"Comment with id {commentId} was not found");
    }

    [Fact]
    public async Task Delete_ThrowsEntityValidationException_When_UserIsNotOwnerOfComment()
    {
        var userId = _fixture.Create<Guid>();
        var comment = _fixture.Create<Comment>();
        _commentRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<CommentByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);

        var act = () => _commentService.Delete(userId, comment.Id);

        await act.Should().ThrowAsync<EntityValidationFailedException>()
            .WithMessage("This comment belongs to another user");
    }

    [Fact]
    public async Task Delete_RemovesComment_When_Success()
    {
        var userId = _fixture.Create<Guid>();
        var comment = _fixture.Create<Comment>();
        _commentRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<CommentByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);
        comment.UserId = userId;

        await _commentService.Delete(userId, comment.Id);

        _commentRepository.Verify(r => r.DeleteAsync(comment, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPagedByPostId_ThrowsEntityValidationException_When_PaginationDtoIsInvalid()
    {
        var postId = _fixture.Create<Guid>();
        var dto = _fixture.Create<PaginationParametersDto>();
        var validationResult = _fixture.Create<ValidationResult>();
        _paginationValidator.Setup(v => v.Validate(dto))
            .Returns(validationResult);
        var expectedException = new EntityValidationFailedException(validationResult.Errors);

        var act = () => _commentService.GetPagedByPostId(postId, dto);

        await act.Should().ThrowAsync<EntityValidationFailedException>()
            .WithMessage(expectedException.Message);
    }

    [Fact]
    public async Task GetPagedByPostId_ReturnsComments_When_Success()
    {
        var postId = _fixture.Create<Guid>();
        var comments = _fixture.CreateMany<Comment>().ToList();
        var dto = _fixture.Create<PaginationParametersDto>();
        _paginationValidator.Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());
        _commentRepository.Setup(r => r.ListAsync(It.IsAny<PagedCommentsByPostIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(comments);


        var result = await _commentService.GetPagedByPostId(postId, dto);

        result.PaginationParameters.Should().Be(dto);
        result.Data.Should().BeEquivalentTo(comments, opt => opt.ExcludingMissingMembers());
    }
}
