namespace BusinessLogic.Dtos;
public record LoginResultDto
{
    public bool IsSuccessful { get; set; }

    public string? ErrorMessage { get; set; }

    public TokensDto? Tokens { get; set; }
}
