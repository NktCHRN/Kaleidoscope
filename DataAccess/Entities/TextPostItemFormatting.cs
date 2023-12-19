using DataAccess.Enums;

namespace DataAccess.Entities;
public class TextPostItemFormatting
{
    public Guid Id { get; set; }
    public int Start {  get; set; }
    public int End { get; set; }
    public TextPostItemFormattingType Formatting { get; set; }

    public Guid TextPostItemId { get; set; }
    public TextPostItem TextPostItem { get; set; } = null!;
}
