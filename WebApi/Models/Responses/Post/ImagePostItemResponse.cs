namespace WebApi.Models.Responses.Post;

public record ImagePostItemResponse : PostItemResponse
{
    public string? Alt { get; set; }
    public string? Description { get; set; }
    public string LocalFileName { get; set; } = string.Empty;
}
