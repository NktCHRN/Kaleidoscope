using AutoMapper;
using BusinessLogic.Abstractions;
using BusinessLogic.Dtos;
using BusinessLogic.Exceptions;
using DataAccess.Abstractions;
using DataAccess.Entities;
using DataAccess.Specifications;
using FluentValidation;

namespace BusinessLogic.Services;
public class CommentService : ICommentService
{
    private readonly IRepository<Comment> _commentRepository;
    private readonly IValidator<CreateCommentDto> _createValidator;
    private readonly IValidator<UpdateCommentDto> _updateValidator;
    private readonly IValidator<PaginationParametersDto> _paginationValidator;
    private readonly IMapper _mapper;
    private readonly IRepository<Post> _postRepository;
    private readonly IRepository<User> _userRepository;
    private readonly TimeProvider _timeProvider;

    public CommentService(IRepository<Comment> commentRepository, IValidator<CreateCommentDto> createValidator, IValidator<UpdateCommentDto> updateValidator, IValidator<PaginationParametersDto> paginationValidator, IMapper mapper, IRepository<Post> postRepository, IRepository<User> userRepository, TimeProvider timeProvider)
    {
        _commentRepository = commentRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _paginationValidator = paginationValidator;
        _mapper = mapper;
        _postRepository = postRepository;
        _userRepository = userRepository;
        _timeProvider = timeProvider;
    }

    public async Task<CommentDto> Create(Guid userId, Guid postId, CreateCommentDto postDto)
    {
        var validationResults = _createValidator.Validate(postDto);
        if (!validationResults.IsValid)
        {
            throw new EntityValidationFailedException(validationResults.Errors);
        }
        var post = await _postRepository.GetByIdAsync(postId) 
            ?? throw new EntityNotFoundException($"Post with id {postId} was not found");
        var user = await _userRepository.FirstOrDefaultAsync(new UserByIdSpec(userId))
            ?? throw new EntityNotFoundException($"User with id {userId} was not found");

        var comment = new Comment
        {
            CreatedAt = _timeProvider.GetUtcNow(),
            Post = post,
            User = user,
            Text = postDto.Text
        };

        await _commentRepository.AddAsync(comment);

        return _mapper.Map<CommentDto>(comment);
    }

    public async Task<CommentDto> Update(Guid userId, Guid commentId, UpdateCommentDto postDto)
    {
        var validationResults = _updateValidator.Validate(postDto);
        if (!validationResults.IsValid)
        {
            throw new EntityValidationFailedException(validationResults.Errors);
        }
        var comment = await _commentRepository.FirstOrDefaultAsync(new CommentByIdSpec(commentId))
            ?? throw new EntityNotFoundException($"Comment with id {commentId} was not found");
        CheckCommentOwnership(userId, comment);

        comment.IsModidied = true;
        comment.Text = postDto.Text;

        await _commentRepository.UpdateAsync(comment);

        return _mapper.Map<CommentDto>(comment);
    }

    public async Task Delete(Guid userId, Guid commentId)
    {
        var comment = await _commentRepository.FirstOrDefaultAsync(new CommentByIdSpec(commentId))
            ?? throw new EntityNotFoundException($"Comment with id {commentId} was not found");
        CheckCommentOwnership(userId, comment);

        await _commentRepository.DeleteAsync(comment);
    }

    private static void CheckCommentOwnership(Guid userId, Comment comment)
    {
        if (comment.UserId != userId)
        {
            throw new EntityValidationFailedException("This comment belongs to another user");
        }
    }

    public async Task<PagedDto<CommentDto, PaginationParametersDto>> GetPagedByPostId(Guid postId, PaginationParametersDto parameters)
    {
        var validationResults = _paginationValidator.Validate(parameters);
        if (!validationResults.IsValid)
        {
            throw new EntityValidationFailedException(validationResults.Errors);
        }

        var posts = await _commentRepository.ListAsync(new PagedCommentsByPostIdSpec(postId, parameters.PerPage, parameters.Page));

        return new PagedDto<CommentDto, PaginationParametersDto>
        {
            Data = _mapper.Map<IEnumerable<CommentDto>>(posts),
            PaginationParameters = parameters,
        };
    }
}
