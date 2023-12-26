namespace WebApi.Models.Requests.User;

public record UpdateUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string? AvatarLocalFileName { get; set; }
}
