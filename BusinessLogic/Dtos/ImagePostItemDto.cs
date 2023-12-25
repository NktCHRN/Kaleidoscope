namespace BusinessLogic.Dtos;
public record ImagePostItemDto : PostItemDto
{
    public string? Alt { get; set; }
    public string? Description { get; set; }
    public string LocalFileName { get; set; } = string.Empty;
}
