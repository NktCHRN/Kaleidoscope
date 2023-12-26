namespace BusinessLogic.Dtos;
public record UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? AvatarLocalFileName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
