namespace WebApi.Models.Responses.User;

public record UserTitleResponse
{
    public string Name { get; set; } = string.Empty;
    public string? AvatarLocalFileName { get; set; }
}
