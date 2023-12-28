using WebApi.IntegrationTests.Stubs;
using WebApi.IntegrationTests.TestDataHelpers;
using Xunit;

namespace WebApi.IntegrationTests.Abstractions;
public class ControllerTestsBase : IAsyncLifetime
{
    protected HttpClient HttpClient;
    protected BlobStorageTestDataHelper BlobStorageTestDataHelper;
    protected DatabaseTestDataHelper DatabaseTestDataHelper;
    protected TestTimeProvider TimeProvider;
    protected TestAuthUser User;

    private readonly CustomWebApplicationFactory<Program> _factory;

    public ControllerTestsBase(CustomWebApplicationFactory<Program> factory)
    {
        HttpClient = factory.HttpClient;
        BlobStorageTestDataHelper = factory.BlobStorageDataHelper;
        DatabaseTestDataHelper = factory.DatabaseTestDataHelper;
        TimeProvider = factory.TestTimeProvider;
        User = factory.TestUser;

        _factory = factory;
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _factory.ResetTestDataAsync();
        _factory.ResetStubs();
    }
}
