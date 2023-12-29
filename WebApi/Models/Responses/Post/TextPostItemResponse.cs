using WebApi.Models.Enums;

namespace WebApi.Models.Responses.Post;

public record TextPostItemResponse : PostItemResponse
{
    public string Text { get; set; } = string.Empty;
    public TextPostType TextPostType { get; set; }
}
