using DataAccess.Enums;

namespace DataAccess.Entities;
public class TextPostItem : PostItem
{
    public string Text { get; set; } = string.Empty;
    public TextPostType TextPostType { get; set; }
}
