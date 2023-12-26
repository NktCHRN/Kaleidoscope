using WebApi.Models.Responses.User;

namespace WebApi.Models.Responses.Comment;

public record CommentResponse
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsModidied { get; set; }
    public UserTitleResponse User { get; set; } = null!;
    public string? UserBlogTag { get; set; }
}
