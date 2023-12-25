using Ardalis.Specification;
using DataAccess.Entities;

namespace DataAccess.Specifications;
public class PagedPostsByBlogIdSpec : Specification<Post>
{
    public PagedPostsByBlogIdSpec(Guid blogId, int perPage, int page)
    {
        Query.AsNoTracking()
            .Include(p => p.Blog)
            .Where(p => p.BlogId == blogId)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * perPage)
            .Take(perPage);
    }
}
