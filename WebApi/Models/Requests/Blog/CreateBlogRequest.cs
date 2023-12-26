namespace WebApi.Models.Requests.Blog;

public record CreateBlogRequest
{
    public string Tag { get; set; } = string.Empty;
    public string? Description { get; set; }
}
