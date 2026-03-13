using Domain.Entities.Briefs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public sealed class BriefItemConfiguration : IEntityTypeConfiguration<BriefItem>
{
    public void Configure(EntityTypeBuilder<BriefItem> builder)
    {
        builder.HasKey(bi => bi.Id);

        builder.Property(bi => bi.Id)
            .HasConversion(id => id.Value, v => new BriefItemId(v));

        builder.Property(bi => bi.BriefId)
            .HasConversion(id => id.Value, v => new BriefId(v))
            .IsRequired();

        builder.Property(bi => bi.SectionType)
            .IsRequired();

        builder.Property(bi => bi.ItemName)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(bi => bi.IsSelected)
            .IsRequired();

        builder.Property(bi => bi.Comments)
            .HasMaxLength(2000);
    }
}
