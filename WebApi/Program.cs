using BusinessLogic.Abstractions;
using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using (var serviceScope = app.Services.CreateScope())
    {
        var roleSeeder = serviceScope.ServiceProvider.GetRequiredService<IRoleSeeder>();
        await roleSeeder.SeedRolesAsync();
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
