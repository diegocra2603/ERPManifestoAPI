using Domain.Entities.Accounting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public sealed class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRate>
{
    public void Configure(EntityTypeBuilder<ExchangeRate> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasConversion(id => id.Value, v => new ExchangeRateId(v));

        builder.Property(e => e.CurrencyId)
            .HasConversion(id => id.Value, v => new CurrencyId(v))
            .IsRequired();

        builder.Property(e => e.Date)
            .IsRequired();

        builder.Property(e => e.BuyRate)
            .HasPrecision(20, 6)
            .IsRequired();

        builder.Property(e => e.SellRate)
            .HasPrecision(20, 6)
            .IsRequired();

        builder.Property(e => e.Source)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(e => new { e.CurrencyId, e.Date })
            .IsUnique();

        builder.HasOne(e => e.Currency)
            .WithMany()
            .HasForeignKey(e => e.CurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsOne(e => e.AuditField, audit =>
        {
            audit.Property(a => a.CreatedAt).IsRequired();
            audit.Property(a => a.UpdatedAt);
            audit.Property(a => a.DeletedAt);
            audit.Property(a => a.IsActive).IsRequired().HasDefaultValue(true);
        });
    }
}
