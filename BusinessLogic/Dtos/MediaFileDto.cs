namespace BusinessLogic.Dtos;
public record MediaFileDto
{
    public string Name { get; set; } = string.Empty;
    public BinaryData Data { get; set; } = null!;
    public string ContentType { get; set; } = string.Empty;
}
