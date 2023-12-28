using System.Data;
using System.Data.Common;
using Azure.Storage.Blobs;
using BusinessLogic.Abstractions;
using DataAccess.Options;
using DataAccess.Persistence;
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
using WebApi.IntegrationTests.TestDataHelpers;
using Xunit;

namespace WebApi.IntegrationTests;
public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
{
    private readonly MsSqlContainer _msSqlContainer
        = new MsSqlBuilder().Build();

    private readonly AzuriteContainer _azuriteContainer = new AzuriteBuilder()
        .WithImage("mcr.microsoft.com/azure-storage/azurite:latest")
        .Build();

    private const string s_blobStorageContainerName = "kaleidoscopetest1";

    public HttpClient HttpClient { get; private set; } = null!;

    private DbConnection? _connection;

    public BlobStorageTestDataHelper BlobStorageDataHelper { get; } = new();
    public DatabaseTestDataHelper DatabaseTestDataHelper { get; } = new();

    private BlobStorageSeeder? _blobStorageSeeder;
    private DatabaseSeeder? _databaseSeeder;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            ConfigureDatabase(services);
            ConfigureAzure(services);
            RemoveRolesSeeder(services);
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

        services.Configure<BlobStorageOptions>(opts => opts.ContainerName = s_blobStorageContainerName);
    }

    private void RemoveRolesSeeder(IServiceCollection services)
    {
        var descriptorType = typeof(IRoleSeeder);

        var descriptor = services
            .SingleOrDefault(s => s.ServiceType == descriptorType);

        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }
    }

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
        await _azuriteContainer.StartAsync();
        HttpClient = CreateClient();

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _connection = dbContext.Database.GetDbConnection();
        if (_connection.State != ConnectionState.Open)
        {
            await _connection.OpenAsync();
        }
        var blobServiceClient = scope.ServiceProvider.GetRequiredService<BlobServiceClient>();

        _blobStorageSeeder = new(blobServiceClient, BlobStorageDataHelper, s_blobStorageContainerName);
        _databaseSeeder = new(dbContext, DatabaseTestDataHelper, _connection);

        await _blobStorageSeeder.SeedAsync();
        await _databaseSeeder.SeedAsync();
    }

    public async Task ResetTestData()
    {
        await _blobStorageSeeder!.RestoreInitialAsync();
        await _databaseSeeder!.RestoreInitialAsync();
    }

    public new async Task DisposeAsync()
    {
        await _msSqlContainer.StopAsync();
        await _azuriteContainer.StopAsync();
        await _connection!.DisposeAsync();
    }
}
