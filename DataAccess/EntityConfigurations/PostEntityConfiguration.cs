using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityConfigurations;
public class PostEntityConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.Property(e => e.NormalizedName)
            .HasMaxLength(256);

        builder.HasIndex(e => new { e.NormalizedName, e.NormalizedNameDuplicatesCount, e.BlogId })
            .IsUnique(true);

        builder.Property(e => e.Header)
            .HasMaxLength(256);

        builder.Property(e => e.Subheader)
            .HasMaxLength(256);
    }
}
