using Ardalis.Specification;
using DataAccess.Entities;

namespace DataAccess.Specifications;
public class CommentByIdSpec : SingleResultSpecification<Comment>
{
    public CommentByIdSpec(Guid id)
    {
        Query.Include(c => c.User)
            .ThenInclude(u => u.Blog)
            .Where(c => c.Id == id);
    }
}
