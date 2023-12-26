namespace BusinessLogic.Dtos;
public record UpdatePostDto
{
    public string Header { get; set; } = string.Empty;
    public string? Subheader { get; set; }
    public IList<PostItemDto> PostItems { get; set; } = new List<PostItemDto>();
}
