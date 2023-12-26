namespace BusinessLogic.Dtos;
public record TokensDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken {  get; set; } = string.Empty;
}
