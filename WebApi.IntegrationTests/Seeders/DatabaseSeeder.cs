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

        await context.Database.MigrateAsync();
        await context.AddRangeAsync(_testDataHelper.GetAllEntities());
        await context.SaveChangesAsync();
        _respawner ??= await Respawner.CreateAsync(context.Database.GetConnectionString()!, new RespawnerOptions{});
    }

    public async Task RestoreInitialAsync(IServiceScope scope)
    {
        var rp1 = _respawner.ReseedSql;
        var rp2 = _respawner.DeleteSql;
        var context = _contextFactory(scope);
        await _respawner!.ResetAsync(context.Database.GetConnectionString()!);
    }
}
