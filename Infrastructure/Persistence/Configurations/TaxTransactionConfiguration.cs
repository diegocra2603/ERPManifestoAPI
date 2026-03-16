using Domain.Entities.Accounting;
using Domain.Entities.FiscalDocuments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public sealed class TaxTransactionConfiguration : IEntityTypeConfiguration<TaxTransaction>
{
    public void Configure(EntityTypeBuilder<TaxTransaction> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasConversion(id => id.Value, v => new TaxTransactionId(v));

        builder.Property(t => t.TaxConfigurationId)
            .HasConversion(id => id.Value, v => new TaxConfigurationId(v))
            .IsRequired();

        builder.Property(t => t.JournalEntryId)
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                v => v.HasValue ? new JournalEntryId(v.Value) : null);

        builder.Property(t => t.FiscalDocumentId)
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                v => v.HasValue ? new FiscalDocumentId(v.Value) : null);

        builder.Property(t => t.CurrencyId)
            .HasConversion(id => id.Value, v => new CurrencyId(v))
            .IsRequired();

        builder.Property(t => t.ExchangeRate)
            .HasPrecision(20, 6)
            .IsRequired();

        builder.Property(t => t.TaxableBase)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(t => t.TaxAmount)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(t => t.TaxableBaseFunctional)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(t => t.TaxAmountFunctional)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(t => t.Date)
            .IsRequired();

        builder.Property(t => t.TransactionType)
            .IsRequired();

        builder.HasOne(t => t.TaxConfiguration)
            .WithMany()
            .HasForeignKey(t => t.TaxConfigurationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.JournalEntry)
            .WithMany()
            .HasForeignKey(t => t.JournalEntryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.FiscalDocument)
            .WithMany()
            .HasForeignKey(t => t.FiscalDocumentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Currency)
            .WithMany()
            .HasForeignKey(t => t.CurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsOne(t => t.AuditField, audit =>
        {
            audit.Property(a => a.CreatedAt).IsRequired();
            audit.Property(a => a.UpdatedAt);
            audit.Property(a => a.DeletedAt);
            audit.Property(a => a.IsActive).IsRequired().HasDefaultValue(true);
        });
    }
}
