using Ardalis.Specification;
using DataAccess.Entities;

namespace DataAccess.Specifications;
public class PostByIdNoTrackingSpec : SingleResultSpecification<Post>
{
    public PostByIdNoTrackingSpec(Guid id)
    {
        Query.AsNoTracking()
            .Include(p => p.Blog)
            .Include(p => p.PostItems.OrderBy(i => i.Order))
            .Where(p => p.Id == id);
    }
}
