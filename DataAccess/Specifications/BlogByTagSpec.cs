using Ardalis.Specification;
using DataAccess.Entities;

namespace DataAccess.Specifications;
public class BlogByTagSpec : SingleResultSpecification<Blog>
{
    public BlogByTagSpec(string tag)
    {
        Query.AsNoTracking().Where(b => b.Tag == tag);
    }
}
