namespace DataAccess.Entities;
public class Comment
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsModidied { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Guid PostId { get; set; }
    public Post Post { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
