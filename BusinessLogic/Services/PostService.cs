using AutoMapper;
using BusinessLogic.Abstractions;
using BusinessLogic.Dtos;
using BusinessLogic.Exceptions;
using DataAccess.Abstractions;
using DataAccess.Entities;
using DataAccess.Specifications;
using FluentValidation;

namespace BusinessLogic.Services;
public class PostService : IPostService
{
    private readonly IRepository<Blog> _blogRepository;
    private readonly IRepository<Post> _postRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreatePostDto> _createValidator;
    private readonly IValidator<UpdatePostDto> _updateValidator;
    private readonly IValidator<PaginationParametersDto> _paginationValidator;

    public PostService(IRepository<Blog> blogRepository, IRepository<Post> postRepository, IMapper mapper, IValidator<CreatePostDto> createValidator, IValidator<UpdatePostDto> updateValidator, IValidator<PaginationParametersDto> paginationValidator)
    {
        _blogRepository = blogRepository;
        _postRepository = postRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _paginationValidator = paginationValidator;
    }

    public async Task<PostDto> Create(Guid userId, Guid blogId, CreatePostDto postDto)
    {
        var validationResults = _createValidator.Validate(postDto);
        if (!validationResults.IsValid)
        {
            throw new EntityValidationFailedException(validationResults.Errors);
        }

        var blog = await _blogRepository.FirstOrDefaultAsync(new BlogByIdSpec(blogId))
            ?? throw new EntityNotFoundException($"Blog with id {blogId} was not found");
        CheckBlogOwnership(userId, blog);

        var post = new Post
        {
            Header = postDto.Header,
            Subheader = postDto.Subheader,
            CreatedAt = DateTimeOffset.UtcNow,
            Blog = blog
        };
        for (var i = 0; i < postDto.PostItems.Count; i++)
        {
            var postItem = _mapper.Map<PostItem>(postDto.PostItems[i]);
            postItem.Id = Guid.Empty;
            postItem.Order = i;
            post.PostItems.Add(postItem);
        }

        await _postRepository.AddAsync(post);

        return _mapper.Map<PostDto>(post);
    }

    public async Task<PostDto> Update(Guid userId, Guid postId, UpdatePostDto postDto)
    {
        var validationResults = _updateValidator.Validate(postDto);
        if (!validationResults.IsValid)
        {
            throw new EntityValidationFailedException(validationResults.Errors);
        }

        var post = await _postRepository.FirstOrDefaultAsync(new PostByIdSpec(postId))
            ?? throw new EntityNotFoundException($"Post with id {postId} was not found");
        CheckBlogOwnership(userId, post.Blog);

        post.Header = postDto.Header;
        post.Subheader = postDto.Subheader;
        post.IsModidied = true;

        var postItemsToDelete = post.PostItems.Where(p => !postDto.PostItems.Any(i => i.Id == p.Id)).ToList();

        foreach (var postItem in postItemsToDelete)
        {
            post.PostItems.Remove(postItem);
        }

        for (var i = 0; i < postDto.PostItems.Count; i++)
        {
            var postItemDto = postDto.PostItems[i];
            if (postItemDto.Id is not null)
            {
                var postItem = post.PostItems.FirstOrDefault(i => i.Id == postItemDto.Id) 
                    ?? throw new EntityNotFoundException($"Post item with id {postItemDto.Id} is not part of the post {post.Id}");
                var postItemId = postItem.Id;
                _mapper.Map(postItemDto, postItem);
                postItem.Id = postItemId;
                postItem.Order = i;
            }
            else
            {
                var postItem = _mapper.Map<PostItem>(postItemDto);
                postItem.Order = i;
                post.PostItems.Add(postItem);
            }
        }

        await _postRepository.UpdateAsync(post);

        post.PostItems = post.PostItems.OrderBy(i => i.Order).ToList();
        return _mapper.Map<PostDto>(post);
    }

    public async Task Delete(Guid userId, Guid postId)
    {
        var post = await _postRepository.FirstOrDefaultAsync(new PostByIdSpec(postId))
            ?? throw new EntityNotFoundException($"Post with id {postId} was not found");
        CheckBlogOwnership(userId, post.Blog);

        await _postRepository.DeleteAsync(post);
    }

    private static void CheckBlogOwnership(Guid userId, Blog blog)
    {
        if (blog.UserId != userId)
        {
            throw new EntityValidationFailedException("This post belongs to another user");
        }
    }

    public async Task<PostDto> GetById(Guid postId)
    {
        var post = await _postRepository.FirstOrDefaultAsync(new PostByIdNoTrackingSpec(postId))
            ?? throw new EntityNotFoundException($"Post with id {postId} was not found.");
        return _mapper.Map<PostDto>(post);
    }

    public async Task<PagedDto<PostTitleDto, PaginationParametersDto>> GetPaged(PaginationParametersDto parameters)
    {
        var validationResults = _paginationValidator.Validate(parameters);
        if (!validationResults.IsValid)
        {
            throw new EntityValidationFailedException(validationResults.Errors);
        }

        var posts = await _postRepository.ListAsync(new PagedPostsSpec(parameters.PerPage, parameters.Page));

        return new PagedDto<PostTitleDto, PaginationParametersDto>
        {
            Data = _mapper.Map<IEnumerable<PostTitleDto>>(posts),
            PaginationParameters = parameters,
        };
    }

    public async Task<PagedDto<PostTitleDto, PaginationParametersDto>> GetPagedByBlogId(Guid blogId, PaginationParametersDto parameters)
    {
        var validationResults = _paginationValidator.Validate(parameters);
        if (!validationResults.IsValid)
        {
            throw new EntityValidationFailedException(validationResults.Errors);
        }

        var posts = await _postRepository.ListAsync(new PagedPostsByBlogIdSpec(blogId, parameters.PerPage, parameters.Page));

        return new PagedDto<PostTitleDto, PaginationParametersDto>
        {
            Data = _mapper.Map<IEnumerable<PostTitleDto>>(posts),
            PaginationParameters = parameters,
        };
    }
}
