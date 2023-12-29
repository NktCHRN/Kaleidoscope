using System.Diagnostics.CodeAnalysis;
using BusinessLogic.Constants;

namespace WebApi.IntegrationTests.Stubs;
public class TestAuthUser
{
    public Guid UserId { get; set; }
    public string UserEmail { get; set; }
    public List<string> Roles { get; set; }
    public bool IsAuthenticated { get; set; }

    public TestAuthUser()
    {
        Reset();
    }

    [MemberNotNull(nameof(UserEmail))]
    [MemberNotNull(nameof(Roles))]
    public void Reset()
    {
        UserId = Guid.Parse("71A5C868-185A-455C-FAC6-08DC05EB70D4");
        UserEmail = "nc@gmail.com";
        Roles = new List<string>
        {
            RolesConstants.RegisteredViewer,
            RolesConstants.Author
        };
        IsAuthenticated = true;
    }
}
