namespace WebApi.Models.Responses.Post;

public record PostResponse
{
    public Guid Id { get; set; }
    public string Header { get; set; } = string.Empty;
    public string? Subheader { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public bool IsModidied { get; set; }
    public string BlogTag { get; set; } = string.Empty;
    public ICollection<PostItemResponse> PostItems { get; set; } = new List<PostItemResponse>();
}
