using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityConfigurations;
public class LikeEntityConfiguration : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        builder.HasIndex(b => new { b.PostId, b.UserId })
            .IsUnique(true);

        builder.ToTable("Likes");

        builder.HasOne(p => p.User)
            .WithMany(u => u.Likes)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}
