namespace WebApi.Models.Requests.Account;

public record RevokeTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
