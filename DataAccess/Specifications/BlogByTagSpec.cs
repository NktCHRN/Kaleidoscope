using Ardalis.Specification;
using DataAccess.Entities;

namespace DataAccess.Specifications;
public class BlogByTagSpec : SingleResultSpecification<Blog>
{
    public BlogByTagSpec(string tag)
    {
        Query.Where(b => b.Tag == tag);
    }
}
