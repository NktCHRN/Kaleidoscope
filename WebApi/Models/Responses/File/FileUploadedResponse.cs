namespace WebApi.Models.Responses.File;

public record FileUploadedResponse
{
    public string FileName { get; set; } = string.Empty;
}
