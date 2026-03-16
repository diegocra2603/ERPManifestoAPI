using Domain.Entities.Accounting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasConversion(id => id.Value, v => new ClientId(v));

        builder.Property(c => c.Name)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(c => c.LegalName)
            .HasMaxLength(500);

        builder.Property(c => c.Nit)
            .HasMaxLength(50);

        builder.HasIndex(c => c.Nit)
            .IsUnique()
            .HasFilter("nit IS NOT NULL");

        builder.Property(c => c.Address)
            .HasMaxLength(500);

        builder.Property(c => c.Phone)
            .HasMaxLength(50);

        builder.Property(c => c.Email)
            .HasMaxLength(256);

        builder.OwnsOne(c => c.AuditField, audit =>
        {
            audit.Property(a => a.CreatedAt).IsRequired();
            audit.Property(a => a.UpdatedAt);
            audit.Property(a => a.DeletedAt);
            audit.Property(a => a.IsActive).IsRequired().HasDefaultValue(true);
        });
    }
}
