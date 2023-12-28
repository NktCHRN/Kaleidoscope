namespace WebApi.IntegrationTests.Abstractions;
public interface ISeeder
{
    Task SeedAsync();
    Task RestoreInitialAsync();
}
