using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityConfigurations;
public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.Property(b => b.Text)
            .HasMaxLength(4000);

        builder.HasOne(p => p.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}
