using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace WebApi.IntegrationTests.Stubs;
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly TestAuthUser _mockAuthUser;

    public const string SchemeName = "TestScheme";

    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, TestAuthUser mockAuthUser)
        : base(options, logger, encoder)
    {
        _mockAuthUser = mockAuthUser;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!_mockAuthUser.IsAuthenticated)
        {
            return Task.FromResult(AuthenticateResult.Fail("Test auth failed"));
        }

        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, _mockAuthUser.UserId.ToString()), 
            new (ClaimTypes.Name, _mockAuthUser.UserEmail),
            new (ClaimTypes.Email, _mockAuthUser.UserEmail)
        };
        foreach (var role in _mockAuthUser.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}
