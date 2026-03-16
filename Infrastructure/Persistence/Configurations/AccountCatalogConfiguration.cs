using Domain.Entities.Accounting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public sealed class AccountCatalogConfiguration : IEntityTypeConfiguration<AccountCatalog>
{
    public void Configure(EntityTypeBuilder<AccountCatalog> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasConversion(id => id.Value, v => new AccountCatalogId(v));

        builder.Property(a => a.AccountCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(a => a.AccountCode)
            .IsUnique();

        builder.Property(a => a.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.AccountType)
            .IsRequired();

        builder.Property(a => a.Nature)
            .IsRequired();

        builder.Property(a => a.ParentId)
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                v => v.HasValue ? new AccountCatalogId(v.Value) : null);

        builder.Property(a => a.Level)
            .IsRequired();

        builder.HasOne(a => a.Parent)
            .WithMany(a => a.Children)
            .HasForeignKey(a => a.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsOne(a => a.AuditField, audit =>
        {
            audit.Property(a => a.CreatedAt).IsRequired();
            audit.Property(a => a.UpdatedAt);
            audit.Property(a => a.DeletedAt);
            audit.Property(a => a.IsActive).IsRequired().HasDefaultValue(true);
        });
    }
}
