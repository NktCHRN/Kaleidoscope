using Ardalis.Specification;
using DataAccess.Entities;

namespace DataAccess.Specifications;
public class UserByIdSpec : SingleResultSpecification<User>
{
    public UserByIdSpec(Guid id)
    {
        Query.Include(u => u.Blog)
            .Where(u => u.Id == id);
    }
}
