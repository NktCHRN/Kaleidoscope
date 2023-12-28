using Azure.Storage.Blobs;
using DataAccess.Options;
using DataAccess.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Testcontainers.Azurite;
using Testcontainers.MsSql;
using WebApi.IntegrationTests.Seeders;
using WebApi.IntegrationTests.Stubs;
using WebApi.IntegrationTests.TestDataHelpers;
using Xunit;

namespace WebApi.IntegrationTests;
public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer
        = new MsSqlBuilder().Build();

    private readonly AzuriteContainer _azuriteContainer = new AzuriteBuilder()
        .WithImage("mcr.microsoft.com/azure-storage/azurite:latest")
        .Build();

    private const string BlobStorageContainerName = "kaleidoscopetest1";

    public HttpClient HttpClient { get; private set; } = null!;

    public BlobStorageTestDataHelper BlobStorageDataHelper { get; } = new();
    public DatabaseTestDataHelper DatabaseTestDataHelper { get; } = new();

    private BlobStorageSeeder? _blobStorageSeeder;
    private DatabaseSeeder? _databaseSeeder;

    public TestTimeProvider TestTimeProvider { get; } = new();
    public TestAuthUser TestUser { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            ConfigureDatabase(services);
            ConfigureAzure(services);
            ConfigureStubs(services);

        });
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["RoleSeederOptions:SeedRoles"] = "false"
                });
        });
    }

    private void ConfigureDatabase(IServiceCollection services)
    {
        var descriptorType =
            typeof(DbContextOptions<ApplicationDbContext>);

        var descriptor = services
            .SingleOrDefault(s => s.ServiceType == descriptorType);

        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(_msSqlContainer.GetConnectionString()));
    }

    private void ConfigureAzure(IServiceCollection services)
    {
        var descriptorTypes = new []
        {
            typeof(AzureEventSourceLogForwarder), typeof(AzureComponentFactory), 
            typeof(IOptionsMonitor<BlobClientOptions>), typeof(IAzureClientFactory<BlobServiceClient>),
            typeof(BlobServiceClient)
        };

        foreach (var descriptorType in descriptorTypes)
        {
            var descriptor = services
                .SingleOrDefault(s => s.ServiceType == descriptorType);

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }
        }

        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(_azuriteContainer.GetConnectionString());
        });

        services.Configure<BlobStorageOptions>(opts => opts.ContainerName = BlobStorageContainerName);
    }

    private void ConfigureStubs(IServiceCollection services)
    {
        var descriptorTypes = new[]
        {
            typeof(TimeProvider)
        };

        foreach (var descriptorType in descriptorTypes)
        {
            var descriptor = services
                .SingleOrDefault(s => s.ServiceType == descriptorType);

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }
        }

        services.AddSingleton<TimeProvider>(_ => TestTimeProvider);
        services.AddSingleton(_ => TestUser);
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                options.DefaultScheme = TestAuthHandler.SchemeName;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                TestAuthHandler.SchemeName, options => { });
    }

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
        await _azuriteContainer.StartAsync();

        _blobStorageSeeder = new(
            scope => scope.ServiceProvider.GetRequiredService<BlobServiceClient>(), 
            BlobStorageDataHelper, 
            BlobStorageContainerName);
        _databaseSeeder = new(scope => scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(), 
            DatabaseTestDataHelper);

        using var scope = Services.CreateScope();
        await _blobStorageSeeder.SeedAsync(scope);
        await _databaseSeeder.SeedAsync(scope);

        HttpClient = CreateClient();
    }

    public async Task ResetTestDataAsync()
    {
        using var scope = Services.CreateScope();
        await _blobStorageSeeder!.RestoreInitialAsync(scope);
        await _databaseSeeder!.RestoreInitialAsync(scope);
    }

    public void ResetStubs()
    {
        TestUser.Reset();
        TestTimeProvider.Reset();
    }

    public new async Task DisposeAsync()
    {
        await _msSqlContainer.StopAsync();
        await _azuriteContainer.StopAsync();
    }
}
