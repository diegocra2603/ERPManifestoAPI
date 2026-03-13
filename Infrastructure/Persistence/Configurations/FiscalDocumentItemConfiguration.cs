using Domain.Entities.FiscalDocuments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public sealed class FiscalDocumentItemEntityConfiguration : IEntityTypeConfiguration<FiscalDocumentItem>
{
    public void Configure(EntityTypeBuilder<FiscalDocumentItem> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasConversion(id => id.Value, v => new FiscalDocumentItemId(v));

        builder.Property(i => i.FiscalDocumentId)
            .HasConversion(id => id.Value, v => new FiscalDocumentId(v))
            .IsRequired();

        builder.Property(i => i.ProductCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(i => i.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(i => i.MeasureUnit)
            .IsRequired();

        builder.Property(i => i.Quantity)
            .HasPrecision(20, 5)
            .IsRequired();

        builder.Property(i => i.Price)
            .HasPrecision(20, 7)
            .IsRequired();

        builder.Property(i => i.DiscountPercentage)
            .HasPrecision(20, 6)
            .IsRequired();

        builder.Property(i => i.GrossAmount)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(i => i.DiscountAmount)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(i => i.ExemptAmount)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(i => i.OtherTaxes)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(i => i.NetAmount)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(i => i.IsrAmount)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(i => i.IvaAmount)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(i => i.TotalAmount)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(i => i.SaleType)
            .IsRequired();
    }
}
