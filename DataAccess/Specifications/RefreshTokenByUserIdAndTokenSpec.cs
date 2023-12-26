using Ardalis.Specification;
using DataAccess.Entities;

namespace DataAccess.Specifications;
public class RefreshTokenByUserIdAndTokenSpec : SingleResultSpecification<RefreshToken>
{
    public RefreshTokenByUserIdAndTokenSpec(Guid userId, string token)
    {
        Query.Where(r => r.UserId == userId && r.Token == token);
    }
}
