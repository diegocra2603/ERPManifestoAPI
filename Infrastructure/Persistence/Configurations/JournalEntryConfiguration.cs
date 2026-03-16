using Domain.Entities.Accounting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public sealed class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.HasKey(j => j.Id);

        builder.Property(j => j.Id)
            .HasConversion(id => id.Value, v => new JournalEntryId(v));

        builder.Property(j => j.EntryNumber)
            .IsRequired();

        builder.HasIndex(j => j.EntryNumber)
            .IsUnique();

        builder.Property(j => j.Date)
            .IsRequired();

        builder.Property(j => j.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(j => j.EntryType)
            .IsRequired();

        builder.Property(j => j.Status)
            .IsRequired();

        builder.Property(j => j.PeriodId)
            .HasConversion(id => id.Value, v => new AccountingPeriodId(v))
            .IsRequired();

        builder.Property(j => j.CurrencyId)
            .HasConversion(id => id.Value, v => new CurrencyId(v))
            .IsRequired();

        builder.Property(j => j.ExchangeRate)
            .HasPrecision(20, 6)
            .IsRequired();

        builder.Ignore(j => j.TotalDebit);
        builder.Ignore(j => j.TotalCredit);
        builder.Ignore(j => j.TotalDebitFunctional);
        builder.Ignore(j => j.TotalCreditFunctional);
        builder.Ignore(j => j.IsBalanced);

        builder.HasOne(j => j.Period)
            .WithMany()
            .HasForeignKey(j => j.PeriodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.Currency)
            .WithMany()
            .HasForeignKey(j => j.CurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(j => j.Lines)
            .WithOne(l => l.JournalEntry)
            .HasForeignKey(l => l.JournalEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(j => j.AuditField, audit =>
        {
            audit.Property(a => a.CreatedAt).IsRequired();
            audit.Property(a => a.UpdatedAt);
            audit.Property(a => a.DeletedAt);
            audit.Property(a => a.IsActive).IsRequired().HasDefaultValue(true);
        });
    }
}
