using DataAccess.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using WebApi.IntegrationTests.Abstractions;
using WebApi.IntegrationTests.TestDataHelpers;

namespace WebApi.IntegrationTests.Seeders;
public class DatabaseSeeder : ISeeder
{
    private readonly Func<IServiceScope, ApplicationDbContext> _contextFactory;
    private readonly DatabaseTestDataHelper _testDataHelper;
    private Respawner? _respawner;

    public DatabaseSeeder(Func<IServiceScope, ApplicationDbContext> contextFactory, DatabaseTestDataHelper testDataHelper)
    {
        _contextFactory = contextFactory;
        _testDataHelper = testDataHelper;
    }

    public async Task SeedAsync(IServiceScope scope)
    {
        var context = _contextFactory(scope);

        await context.AddRangeAsync(_testDataHelper.GetAllEntities());
        await context.SaveChangesAsync();

        var connection = context.Database.GetDbConnection();
        await connection.OpenAsync();
        _respawner ??= await Respawner.CreateAsync(connection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.SqlServer,
                SchemasToInclude = ["dbo"]
            });
    }

    public async Task RestoreInitialAsync(IServiceScope scope)
    {
        var context = _contextFactory(scope);
        var connection = context.Database.GetDbConnection();
        await connection.OpenAsync();
        await _respawner!.ResetAsync(connection);
    }
}
