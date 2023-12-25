namespace WebApi.Models.Requests.Comment;

public record UpdateCommentRequest
{
    public string Text { get; set; } = string.Empty;
}
