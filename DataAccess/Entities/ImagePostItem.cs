namespace DataAccess.Entities;
public class ImagePostItem : PostItem
{
    public string? Alt { get; set; }
    public string? Description { get; set; }
    public string InitialFileName { get; set; } = string.Empty;
    public string FileId { get; set; } = string.Empty;
}
