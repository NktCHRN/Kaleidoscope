namespace WebApi.Models.Responses.User;

public record UserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? AvatarFileName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
