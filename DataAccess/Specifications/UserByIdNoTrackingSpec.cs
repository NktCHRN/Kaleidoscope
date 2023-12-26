using Ardalis.Specification;
using DataAccess.Entities;

namespace DataAccess.Specifications;
public class UserByIdNoTrackingSpec : SingleResultSpecification<User>
{
    public UserByIdNoTrackingSpec(Guid id)
    {
        Query.AsNoTracking()
            .Where(u => u.Id == id);
    }
}
