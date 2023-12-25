using Microsoft.AspNetCore.Identity;

namespace DataAccess.Entities;
public class User : IdentityUser<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string? AvatarLocalFileName { get; set; }
    public DateTime CreatedAt { get; set; }

    public Blog? Blog { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
