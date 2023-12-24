using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityConfigurations;
public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.HasIndex(b => new { b.BlogId, b.SubscriberId })
            .IsUnique(true);

        builder
            .HasOne(s => s.Subscriber)
            .WithMany(u => u.Subscriptions)
            .HasForeignKey(s => s.SubscriberId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(true);
    }
}
