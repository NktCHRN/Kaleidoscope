namespace BusinessLogic.Dtos;
public record PostDto
{
    public Guid Id { get; set; }
    public string Header { get; set; } = string.Empty;
    public string? Subheader { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsModidied { get; set; }
    public string BlogTag { get; set; } = string.Empty;
    public ICollection<PostItemDto> PostItems { get; set; } = new List<PostItemDto>();
}
