namespace DataAccess.Entities;
public class PostItem
{
    public Guid Id { get; set; }

    public Guid PostId { get; set; }
    public Post Post { get; set; } = null!;
}
