namespace DataAccess.Entities;
public class ImagePostItem : PostItem
{
    public string? Alt { get; set; }
    public string? Description { get; set; }
    public string LocalFileName { get; set; } = string.Empty;
}
