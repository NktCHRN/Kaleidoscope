using System.Security.Claims;

namespace BusinessLogic.Abstractions;
public interface IJwtTokenProvider
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    public string GenerateRefreshToken();
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
