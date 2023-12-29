using Microsoft.Extensions.DependencyInjection;

namespace WebApi.IntegrationTests.Abstractions;
public interface ISeeder
{
    Task SeedAsync(IServiceScope scope);
    Task RestoreInitialAsync(IServiceScope scope);
}
