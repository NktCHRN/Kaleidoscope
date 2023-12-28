using WebApi.IntegrationTests.Abstractions;
using Xunit;

namespace WebApi.IntegrationTests.Tests;
[Collection("Test collection")]
public class AccountControllerTests : ControllerTestsBase
{
    public AccountControllerTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
        var temp = factory.HttpClient;
    }

    [Fact]
    public void Test1()
    {

    }

    [Fact]
    public void Test2()
    {

    }
}
