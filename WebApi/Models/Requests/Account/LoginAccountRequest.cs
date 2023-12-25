namespace WebApi.Models.Requests.Account;

public record LoginAccountRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
