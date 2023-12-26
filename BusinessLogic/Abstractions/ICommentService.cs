using BusinessLogic.Dtos;

namespace BusinessLogic.Abstractions;
public interface ICommentService
{
    Task<CommentDto> Create(Guid userId, Guid postId, CreateCommentDto postDto);
    Task<CommentDto> Update(Guid userId, Guid commentId, UpdateCommentDto postDto);
    Task Delete(Guid userId, Guid commentId);
    Task<PagedDto<CommentDto, PaginationParametersDto>> GetPagedByPostId(Guid postId, PaginationParametersDto parameters);
}
