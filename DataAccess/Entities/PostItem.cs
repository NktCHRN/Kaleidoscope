namespace DataAccess.Entities;
public abstract class PostItem
{
    public Guid Id { get; set; }
    public int Order { get; set; }

    public Guid PostId { get; set; }
    public Post Post { get; set; } = null!;
}
