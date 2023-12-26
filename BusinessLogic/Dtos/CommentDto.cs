namespace BusinessLogic.Dtos;
public record CommentDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsModidied { get; set; }
    public UserTitleDto User { get; set; } = null!;
    public string? UserBlogTag { get; set; }
}
