namespace WebApi.Models.Responses.Post;

public record TextPostItemResponse : PostItemResponse
{
    public string Text { get; set; } = string.Empty;
    public string TextPostType { get; set; } = string.Empty;
}
