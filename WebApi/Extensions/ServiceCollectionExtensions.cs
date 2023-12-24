using BusinessLogic.Abstractions;
using BusinessLogic.Options;
using BusinessLogic.Seeders;
using DataAccess.Abstractions;
using DataAccess.Entities;
using DataAccess.Options;
using DataAccess.Persistence;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddOptions(configuration)
            .AddDatabase(configuration)
            .AddAzureServices(configuration)
            .AddRepositories()
            .AddSeeders()
            .AddAuth(configuration)
            .AddApiControllers()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();
    }

    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        return services.Configure<BlobStorageOptions>(configuration.GetSection("BlobStorageOptions"))
            .Configure<JwtBearerConfigOptions>(configuration.GetSection("JwtBearer"));
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DbConnectionString")));
    }

    private static IServiceCollection AddAzureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(configuration.GetConnectionString("BlobStorageConnectionString"));
        });
        return services;

    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>))
            .AddSingleton<IBlobRepository, BlobRepository>();
    }

    private static IServiceCollection AddSeeders(this IServiceCollection services)
    {
        return services.AddScoped<IRoleSeeder, RoleSeeder>();
    }

    private static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<User, IdentityRole<Guid>>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
            //options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";
            //options.Tokens.PasswordResetTokenProvider = "CustomPasswordReset";
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        //.AddTokenProvider<CustomEmailConfirmationTokenProvider<User>>("CustomEmailConfirmation")
        //.AddTokenProvider<CustomPasswordResetTokenProvider<User>>("CustomPasswordReset");

        // TODO: + Add jwt

        return services;
    }

    private static IServiceCollection AddApiControllers(this IServiceCollection services)
    {
        services.AddControllers();
        return services;
    }
}
