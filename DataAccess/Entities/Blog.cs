namespace DataAccess.Entities;
public class Blog
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? AvatarId { get; set; }
    public string Tag { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Post> Posts { get; set; } = new List<Post>();

    public ICollection<Subscription> Subscribers { get; set; } = new List<Subscription>();

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
