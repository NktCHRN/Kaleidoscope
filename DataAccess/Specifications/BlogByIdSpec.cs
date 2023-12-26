using Ardalis.Specification;
using DataAccess.Entities;

namespace DataAccess.Specifications;
public class BlogByIdSpec : SingleResultSpecification<Blog>
{
    public BlogByIdSpec(Guid id)
    {
        Query
            .Include(b => b.User)
            .Where(b => b.Id == id);
    }
}
