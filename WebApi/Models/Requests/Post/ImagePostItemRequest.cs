namespace WebApi.Models.Requests.Post;

public record ImagePostItemRequest : PostItemRequest
{
    public string? Alt { get; set; }
    public string? Description { get; set; }
    public string LocalFileName { get; set; } = string.Empty;
}
