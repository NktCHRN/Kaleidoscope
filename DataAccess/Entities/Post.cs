namespace DataAccess.Entities;
public class Post
{
    public Guid Id { get; set; }
    public string Header { get; set; } = string.Empty;
    public string? Subheader {  get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public bool IsModidied { get; set; }

    public Guid BlogId { get; set; }
    public Blog Blog { get; set; } = null!;
    public IList<PostItem> PostItems { get; set; } = new List<PostItem>();
    public IList<Comment> Comments { get; set; } = new List<Comment>();
}
