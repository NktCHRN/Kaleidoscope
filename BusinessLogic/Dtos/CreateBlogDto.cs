namespace BusinessLogic.Dtos;
public record CreateBlogDto
{
    public string Tag { get; set; } = string.Empty;
    public string? Description { get; set; }
}
