namespace DataAccess.Entities;
public class Subscription
{
    public Guid Id { get; set; }

    public Guid BlogId { get; set; }
    public Blog Blog { get; set; } = null!;
    public Guid SubscriberId { get; set; }
    public User Subscriber { get; set; } = null!;
}
