using Microsoft.AspNetCore.Identity;

namespace DataAccess.Entities;
public class User : IdentityUser<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public Guid AvatarId { get; set; }
}
