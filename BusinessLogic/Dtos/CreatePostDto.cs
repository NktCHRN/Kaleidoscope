namespace BusinessLogic.Dtos;
public record CreatePostDto
{
    public string Header { get; set; } = string.Empty;
    public string? Subheader { get; set; }
    public ICollection<PostItemDto> PostItems { get; set; } = new List<PostItemDto>();
}
