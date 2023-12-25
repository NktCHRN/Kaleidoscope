using Ardalis.Specification;
using DataAccess.Entities;

namespace DataAccess.Specifications;
public class PagedPostsSpec : Specification<Post>
{
    public PagedPostsSpec(int perPage, int page) 
    {
        Query.AsNoTracking()
            .Include(p => p.Blog)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * perPage)
            .Take(perPage);
    }
}
