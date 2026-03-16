using Domain.Entities.Accounting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasConversion(id => id.Value, v => new InvoiceId(v));

        builder.Property(i => i.InvoiceType)
            .IsRequired();

        builder.Property(i => i.Status)
            .IsRequired();

        builder.Property(i => i.InvoiceNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(i => i.InvoiceNumber)
            .IsUnique();

        builder.Property(i => i.Date)
            .IsRequired();

        builder.Property(i => i.Nit)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.Name)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(i => i.Address)
            .HasMaxLength(500);

        builder.Property(i => i.Notes)
            .HasMaxLength(1000);

        builder.Property(i => i.FiscalSerie)
            .HasMaxLength(50);

        builder.Property(i => i.FiscalNumero)
            .HasMaxLength(50);

        builder.Property(i => i.FiscalAutorizacion)
            .HasMaxLength(100);

        builder.Property(i => i.ClientId)
            .HasConversion(
                id => id == null ? (Guid?)null : id.Value,
                v => v == null ? null : new ClientId(v.Value));

        builder.Property(i => i.SupplierId)
            .HasConversion(
                id => id == null ? (Guid?)null : id.Value,
                v => v == null ? null : new SupplierId(v.Value));

        builder.Property(i => i.CurrencyId)
            .HasConversion(id => id.Value, v => new CurrencyId(v))
            .IsRequired();

        builder.Property(i => i.JournalEntryId)
            .HasConversion(
                id => id == null ? (Guid?)null : id.Value,
                v => v == null ? null : new JournalEntryId(v.Value));

        builder.Property(i => i.ExchangeRate)
            .HasPrecision(20, 6)
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

        builder.HasOne(i => i.Client)
            .WithMany()
            .HasForeignKey(i => i.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Supplier)
            .WithMany()
            .HasForeignKey(i => i.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Currency)
            .WithMany()
            .HasForeignKey(i => i.CurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.JournalEntry)
            .WithMany()
            .HasForeignKey(i => i.JournalEntryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.Items)
            .WithOne(item => item.Invoice)
            .HasForeignKey(item => item.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(i => i.AuditField, audit =>
        {
            audit.Property(a => a.CreatedAt).IsRequired();
            audit.Property(a => a.UpdatedAt);
            audit.Property(a => a.DeletedAt);
            audit.Property(a => a.IsActive).IsRequired().HasDefaultValue(true);
        });
    }
}
