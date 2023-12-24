using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityConfigurations;
public class BlogEntityConfiguration : IEntityTypeConfiguration<Blog>
{
    public void Configure(EntityTypeBuilder<Blog> builder)
    {
        builder.HasIndex(e => e.Tag)
            .IsUnique(true);

        builder.Property(e => e.Name)
            .HasMaxLength(256);

        builder.Property(e => e.Tag)
            .HasMaxLength(128);

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.AvatarLocalFileName)
            .HasMaxLength(128);
    }
}
