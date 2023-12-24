using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityConfigurations;
internal class TextPostItemFormattingConfiguration : IEntityTypeConfiguration<TextPostItemFormatting>
{
    public void Configure(EntityTypeBuilder<TextPostItemFormatting> builder)
    {
        builder.ToTable("TextPostItemFormattings");

        builder.Property(e => e.Formatting)
            .HasMaxLength(128);
    }
}
