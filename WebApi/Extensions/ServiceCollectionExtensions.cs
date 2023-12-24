using DataAccess.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddDatabase(configuration)
            .AddApiControllers()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DbConnectionString")));
    }

    public static IServiceCollection AddApiControllers(this IServiceCollection services)
    {
        services.AddControllers();
        return services;
    }
}
