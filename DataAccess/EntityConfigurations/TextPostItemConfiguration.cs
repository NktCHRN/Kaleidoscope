using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityConfigurations;
public class TextPostItemConfiguration : IEntityTypeConfiguration<TextPostItem>
{
    public void Configure(EntityTypeBuilder<TextPostItem> builder)
    {
        builder.Property(e => e.TextPostType)
            .HasMaxLength(128);
    }
}
