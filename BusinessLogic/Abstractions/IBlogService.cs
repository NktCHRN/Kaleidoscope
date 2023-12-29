using BusinessLogic.Dtos;

namespace BusinessLogic.Abstractions;
public interface IBlogService
{
    Task<BlogDto> Create(Guid userId, CreateBlogDto createBlogDto);
    Task<BlogDto> Update(Guid userId, Guid blogId, UpdateBlogDto updateBlogDto);
    Task<BlogDto> GetByTag(string tag);
}
