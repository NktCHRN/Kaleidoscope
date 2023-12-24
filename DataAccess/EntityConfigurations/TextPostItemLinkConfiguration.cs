using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityConfigurations;
public class TextPostItemLinkConfiguration : IEntityTypeConfiguration<TextPostItemLink>
{
    public void Configure(EntityTypeBuilder<TextPostItemLink> builder)
    {
        builder.Property(e => e.Url)
            .HasMaxLength(256);

        builder.ToTable("TextPostItemLinks");
    }
}
