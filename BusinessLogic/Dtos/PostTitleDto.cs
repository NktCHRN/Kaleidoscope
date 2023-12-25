namespace BusinessLogic.Dtos;
public record PostTitleDto
{
    public Guid Id { get; set; }
    public string Header { get; set; } = string.Empty;
    public string? Subheader { get; set; }
    public DateTime CreatedAt { get; set; }
    public BlogTitleDto Blog { get; set; } = null!;
}
