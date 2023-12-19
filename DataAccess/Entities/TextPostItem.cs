using DataAccess.Enums;

namespace DataAccess.Entities;
public class TextPostItem : PostItem
{
    public string Text { get; set; } = string.Empty;
    public TextPostType TextPostType { get; set; }

    public ICollection<TextPostItemFormatting> Formattings { get; set; } = new List<TextPostItemFormatting>();
    public ICollection<TextPostItemLink> Links { get; set; } = new List<TextPostItemLink>();
}
