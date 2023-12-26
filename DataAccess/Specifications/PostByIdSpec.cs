using Ardalis.Specification;
using DataAccess.Entities;

namespace DataAccess.Specifications;
public class PostByIdSpec : SingleResultSpecification<Post>
{
    public PostByIdSpec(Guid id) 
    {
        Query.Include(p => p.Blog)
            .Include(p => p.PostItems.OrderBy(i => i.Order))
            .Where(p => p.Id == id);
    }
}
