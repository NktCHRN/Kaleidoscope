using DataAccess.Enums;

namespace BusinessLogic.Dtos;
public record TextPostItemDto : PostItemDto
{
    public string Text { get; set; } = string.Empty;
    public TextPostType TextPostType { get; set; }
}
