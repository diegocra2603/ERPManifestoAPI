using Domain.Entities.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(id => id.Value, v => new PermissionId(v));

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

        SeedPermissions(builder);
    }

    private static void SeedPermissions(EntityTypeBuilder<Permission> builder)
    {
        var permissions = PermissionConstants.GetAll();

        foreach (var permission in permissions)
        {
            builder.HasData(new
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description,
                Code = permission.Code
            });

            // Seed del owned type AuditField
            builder.OwnsOne(p => p.AuditField).HasData(new
            {
                PermissionId = permission.Id,
                CreatedAt = permission.AuditField.CreatedAt,
                UpdatedAt = permission.AuditField.UpdatedAt,
                DeletedAt = permission.AuditField.DeletedAt,
                IsActive = permission.AuditField.IsActive
            });
        }
    }
}
