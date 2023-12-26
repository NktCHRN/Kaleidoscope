namespace WebApi.Models.Responses.Blog;

public record BlogTitleResponse
{
    public string Name { get; set; } = string.Empty;
    public string? AvatarLocalFileName { get; set; }
    public string Tag { get; set; } = string.Empty;
}
