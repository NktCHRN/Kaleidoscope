namespace BusinessLogic.Dtos;
public record UserTitleDto
{
    public string Name { get; set; } = string.Empty;
    public string? AvatarLocalFileName { get; set; }
}
