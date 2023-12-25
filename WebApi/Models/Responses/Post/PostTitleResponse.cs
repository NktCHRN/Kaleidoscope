using WebApi.Models.Responses.Blog;

namespace WebApi.Models.Responses.Post;

public class PostTitleResponse
{
    public Guid Id { get; set; }
    public string Header { get; set; } = string.Empty;
    public string? Subheader { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public BlogTitleResponse Blog { get; set; } = null!;
}
