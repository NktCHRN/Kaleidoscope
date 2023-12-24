using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityConfigurations;
public class PostItemConfiguration : IEntityTypeConfiguration<PostItem>
{
    public void Configure(EntityTypeBuilder<PostItem> builder)
    {
        builder.HasDiscriminator<string>("Discriminator")
            .HasValue<TextPostItem>(nameof(TextPostItem))
            .HasValue<ImagePostItem>(nameof(ImagePostItem));

        builder.ToTable("PostItems");
    }
}
