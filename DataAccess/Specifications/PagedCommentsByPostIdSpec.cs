using Ardalis.Specification;
using DataAccess.Entities;

namespace DataAccess.Specifications;
public class PagedCommentsByPostIdSpec : Specification<Comment>
{
    public PagedCommentsByPostIdSpec(Guid postId, int perPage, int page)
    {
        Query.AsNoTracking()
            .Include(p => p.User)
                .ThenInclude(u => u.Blog)
            .Where(p => p.PostId == postId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * perPage)
            .Take(perPage);
    }
}
