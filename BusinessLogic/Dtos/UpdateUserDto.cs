namespace BusinessLogic.Dtos;
public record UpdateUserDto
{
    public string Name { get; set; } = string.Empty;
    public string? AvatarLocalFileName { get; set; }
}
