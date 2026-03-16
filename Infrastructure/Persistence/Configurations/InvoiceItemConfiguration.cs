using Domain.Entities.Accounting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public sealed class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasConversion(id => id.Value, v => new InvoiceItemId(v));

        builder.Property(i => i.InvoiceId)
            .HasConversion(id => id.Value, v => new InvoiceId(v))
            .IsRequired();

        builder.Property(i => i.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(i => i.Quantity)
            .HasPrecision(20, 5)
            .IsRequired();

        builder.Property(i => i.UnitPrice)
            .HasPrecision(20, 7)
            .IsRequired();

        builder.Property(i => i.Subtotal)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(i => i.TaxAmount)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(i => i.Total)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(i => i.LineOrder)
            .IsRequired();
    }
}
