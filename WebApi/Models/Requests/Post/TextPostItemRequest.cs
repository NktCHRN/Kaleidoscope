using WebApi.Models.Enums;

namespace WebApi.Models.Requests.Post;

public record TextPostItemRequest : PostItemRequest
{
    public string Text { get; set; } = string.Empty;
    public TextPostType TextPostType { get; set; }
}
