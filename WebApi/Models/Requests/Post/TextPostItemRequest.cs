namespace WebApi.Models.Requests.Post;

public record TextPostItemRequest : PostItemRequest
{
    public string Text { get; set; } = string.Empty;
    public string TextPostType { get; set; } = string.Empty;
}
