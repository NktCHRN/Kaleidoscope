namespace WebApi.Models.Responses.Common;

public record ErrorResponse
{
    public string ErrorMessage { get; set; } = string.Empty;
}
