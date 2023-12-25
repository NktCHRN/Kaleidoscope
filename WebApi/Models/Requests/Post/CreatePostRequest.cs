namespace WebApi.Models.Requests.Post;

public record CreatePostRequest
{
    public string Header { get; set; } = string.Empty;
    public string? Subheader { get; set; }
    public IList<PostItemRequest> PostItems { get; set; } = new List<PostItemRequest>();
}
