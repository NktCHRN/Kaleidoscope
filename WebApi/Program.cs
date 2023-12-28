using BusinessLogic.Abstractions;
using WebApi.Extensions;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    if (bool.Parse(builder.Configuration["RoleSeederOptions:SeedRoles"]!))
    {
        using (var serviceScope = app.Services.CreateScope())
        {
            var roleSeeder = serviceScope.ServiceProvider.GetRequiredService<IRoleSeeder>();
            await roleSeeder.SeedRolesAsync();
        }
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();

public partial class Program
{

}
