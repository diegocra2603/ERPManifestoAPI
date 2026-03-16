using Domain.Entities.Accounting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public sealed class TaxConfigurationConfiguration : IEntityTypeConfiguration<TaxConfiguration>
{
    public void Configure(EntityTypeBuilder<TaxConfiguration> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasConversion(id => id.Value, v => new TaxConfigurationId(v));

        builder.Property(t => t.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.Percentage)
            .HasPrecision(10, 4)
            .IsRequired();

        builder.Property(t => t.TaxType)
            .IsRequired();

        builder.Property(t => t.DebitAccountId)
            .HasConversion(id => id.Value, v => new AccountCatalogId(v))
            .IsRequired();

        builder.Property(t => t.CreditAccountId)
            .HasConversion(id => id.Value, v => new AccountCatalogId(v))
            .IsRequired();

        builder.HasOne(t => t.DebitAccount)
            .WithMany()
            .HasForeignKey(t => t.DebitAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.CreditAccount)
            .WithMany()
            .HasForeignKey(t => t.CreditAccountId)
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
