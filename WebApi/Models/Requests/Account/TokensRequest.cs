namespace WebApi.Models.Requests.Account;

public record TokensRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
