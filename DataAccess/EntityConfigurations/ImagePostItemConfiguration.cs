using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityConfigurations;
public class ImagePostItemConfiguration : IEntityTypeConfiguration<ImagePostItem>
{
    public void Configure(EntityTypeBuilder<ImagePostItem> builder)
    {
        builder.Property(e => e.Alt)
            .HasMaxLength(256);

        builder.Property(e => e.Description)
            .HasMaxLength(512);

        builder.Property(e => e.InitialFileName)
            .HasMaxLength(256);
    }
}
