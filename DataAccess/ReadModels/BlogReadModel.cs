namespace DataAccess.ReadModels;
public sealed class BlogReadModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? AvatarLocalFileName { get; set; }
    public string Tag { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }


    public int SubscribersCount { get; set; }
}
