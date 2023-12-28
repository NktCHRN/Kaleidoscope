using Xunit;

namespace WebApi.IntegrationTests.Tests;
[Collection("Test collection")]
public class AccountControllerTests : IAsyncLifetime
{
    public AccountControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        var temp = factory.HttpClient;
    }

    [Fact]
    public void Test1()
    {

    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }


    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
