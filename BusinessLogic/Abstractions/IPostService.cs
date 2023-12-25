using BusinessLogic.Dtos;

namespace BusinessLogic.Abstractions;
public interface IPostService
{
    Task<PostDto> Create(Guid userId, Guid blogId, CreatePostDto postDto);
    Task<PostDto> Update(Guid userId, Guid postId, UpdatePostDto postDto);
    Task Delete(Guid userId, Guid postId);
}
