namespace DataAccess.Entities;
public class Post
{
    public Guid Id { get; set; }
    public string Header { get; set; } = string.Empty;
    public string? Subheader {  get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsModidied { get; set; }
    public string NormalizedName { get; set; } = string.Empty;
    public int NormalizedNameDuplicatesCount { get; set; }

    public Guid BlogId { get; set; }
    public Blog Blog { get; set; } = null!;
    public ICollection<PostItem> PostItems { get; set; } = new List<PostItem>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
