using System.Data;
using System.Data.Common;
using DataAccess.Persistence;
using Respawn;
using WebApi.IntegrationTests.Abstractions;
using WebApi.IntegrationTests.TestDataHelpers;

namespace WebApi.IntegrationTests.Seeders;
public class DatabaseSeeder : ISeeder
{
    private readonly ApplicationDbContext _context;
    private readonly DatabaseTestDataHelper _testDataHelper;
    private Respawner? _respawner;
    private readonly DbConnection _connection;

    public DatabaseSeeder(ApplicationDbContext context, DatabaseTestDataHelper testDataHelper, DbConnection dbConnection)
    {
        _context = context;
        _testDataHelper = testDataHelper;

        _connection = dbConnection;
    }

    public async Task SeedAsync()
    {
        await _context.AddRangeAsync(_testDataHelper.GetAllEntities());
        await _context.SaveChangesAsync();

        _respawner ??= await Respawner.CreateAsync(_connection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.SqlServer,
                SchemasToInclude = ["dbo"]
            });
    }

    public async Task RestoreInitialAsync()
    {
        await _respawner!.ResetAsync(_connection);
    }
}
