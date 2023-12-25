namespace WebApi.Models.Requests.Comment;

public record CreateCommentRequest
{
    public string Text { get; set; } = string.Empty;
}
