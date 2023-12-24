using BusinessLogic.Abstractions;
using BusinessLogic.Constants;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Seeders;
public sealed class RoleSeeder : IRoleSeeder
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IReadOnlyCollection<string> _roleNames = new List<string> {RolesConstants.RegisteredViewer, RolesConstants.Author };

    public RoleSeeder(RoleManager<IdentityRole<Guid>> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task SeedRolesAsync()
    {
        foreach (var roleName in _roleNames)
        {
            var existingRole = await _roleManager.FindByNameAsync(roleName);
            if (existingRole is null)
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
        }
    }
}
