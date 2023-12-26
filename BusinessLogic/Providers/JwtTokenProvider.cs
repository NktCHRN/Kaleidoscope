using BusinessLogic.Abstractions;
using BusinessLogic.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BusinessLogic.Providers;
public class JwtTokenProvider : IJwtTokenProvider
{
    private readonly IOptions<JwtBearerConfigOptions> _options;
    private readonly TimeProvider _timeProvider;

    public JwtTokenProvider(IOptions<JwtBearerConfigOptions> options, TimeProvider timeProvider)
    {
        _options = options;
        _timeProvider = timeProvider;
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var jwt = new JwtSecurityToken(
                issuer: _options.Value.Issuer,
                audience: _options.Value.Audience,
                notBefore: now,
                claims: claims,
                expires: now.Add(TimeSpan.FromMinutes(_options.Value.LifeTime)),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(_options.Value.Secret)),
                    SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _options.Value.Issuer,

            ValidateAudience = true,
            ValidAudience = _options.Value.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_options.Value.Secret)),
            ValidateIssuerSigningKey = true,

            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");
        return principal;
    }
}
