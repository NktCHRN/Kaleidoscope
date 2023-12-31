﻿namespace DataAccess.Entities;
public class Blog
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? AvatarLocalFileName { get; set; }
    public string Tag { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public IList<Post> Posts { get; set; } = new List<Post>();

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
