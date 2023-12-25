using AutoMapper;
using BusinessLogic.Abstractions;
using BusinessLogic.Dtos;
using BusinessLogic.Exceptions;
using DataAccess.Abstractions;
using DataAccess.Entities;
using DataAccess.Specifications;

namespace BusinessLogic.Services;
public class PostService : IPostService
{
    private readonly IRepository<Blog> _blogRepository;
    private readonly IRepository<Post> _postRepository;
    private readonly IMapper _mapper;

    public PostService(IRepository<Blog> blogRepository, IRepository<Post> postRepository, IMapper mapper)
    {
        _blogRepository = blogRepository;
        _postRepository = postRepository;
        _mapper = mapper;
    }

    public async Task<PostDto> Create(Guid userId, Guid blogId, CreatePostDto postDto)
    {
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
            var postItem = _mapper.Map<PostItem>(postDto);
            postItem.Order = i;
            post.PostItems.Add(postItem);
        }

        await _postRepository.AddAsync(post);

        return _mapper.Map<PostDto>(postDto);
    }

    public Task<PostDto> Update(Guid userId, Guid postId, UpdatePostDto postDto)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Guid userId, Guid postId)
    {
        throw new NotImplementedException();
    }

    private static void CheckBlogOwnership(Guid userId, Blog blog)
    {
        if (blog.UserId != userId)
        {
            throw new EntityValidationFailedException("This post belongs to another user");
        }
    }
}
