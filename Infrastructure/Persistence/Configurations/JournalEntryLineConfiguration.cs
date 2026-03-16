using Domain.Entities.Accounting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public sealed class JournalEntryLineConfiguration : IEntityTypeConfiguration<JournalEntryLine>
{
    public void Configure(EntityTypeBuilder<JournalEntryLine> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .HasConversion(id => id.Value, v => new JournalEntryLineId(v));

        builder.Property(l => l.JournalEntryId)
            .HasConversion(id => id.Value, v => new JournalEntryId(v))
            .IsRequired();

        builder.Property(l => l.AccountId)
            .HasConversion(id => id.Value, v => new AccountCatalogId(v))
            .IsRequired();

        builder.Property(l => l.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(l => l.Debit)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(l => l.Credit)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(l => l.DebitFunctional)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(l => l.CreditFunctional)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(l => l.LineOrder)
            .IsRequired();

        builder.HasOne(l => l.Account)
            .WithMany()
            .HasForeignKey(l => l.AccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
