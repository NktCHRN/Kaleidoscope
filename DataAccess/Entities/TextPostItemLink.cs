namespace DataAccess.Entities;
public class TextPostItemLink
{
    public Guid Id { get; set; }
    public int Start { get; set; }
    public int End { get; set; }
    public string Url { get; set; } = string.Empty;

    public Guid TextPostItemId { get; set; }
    public TextPostItem TextPostItem { get; set; } = null!;
}
