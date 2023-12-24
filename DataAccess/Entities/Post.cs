namespace DataAccess.Entities;
public class Post
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsModidied { get; set; }

    public Guid BlogId { get; set; }
    public Blog Blog { get; set; } = null!;
    public ICollection<PostItem> PostItems { get; set; } = new List<PostItem>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
}
