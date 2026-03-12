using Domain.Entities.FiscalData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public sealed class FiscalDataEntryConfiguration : IEntityTypeConfiguration<FiscalDataEntry>
{
    public void Configure(EntityTypeBuilder<FiscalDataEntry> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .HasConversion(id => id.Value, v => new FiscalDataEntryId(v));

        builder.Property(f => f.FiscalCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(f => f.FiscalCode)
            .IsUnique();

        builder.Property(f => f.FiscalName)
            .HasMaxLength(500)
            .IsRequired();

        builder.OwnsOne(t => t.AuditField, audit =>
        {
            audit.Property(a => a.CreatedAt)
                .IsRequired();

            audit.Property(a => a.UpdatedAt);

            audit.Property(a => a.DeletedAt);

            audit.Property(a => a.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
        });
    }
}
