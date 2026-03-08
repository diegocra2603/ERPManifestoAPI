using Domain.Entities.Permissions;
using Domain.Entities.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasConversion(id => id.Value, v => new RoleId(v));

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

        // Relación many-to-many mediante la entidad RolePermission (tabla RolePermissions)
        builder.HasMany(r => r.Permissions)
            .WithMany()
            .UsingEntity<RolePermission>(
                j => j.HasOne(rp => rp.Permission)
                    .WithMany()
                    .HasForeignKey(rp => rp.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne(rp => rp.Role)
                    .WithMany()
                    .HasForeignKey(rp => rp.RoleId)
                    .OnDelete(DeleteBehavior.Cascade));

        // Seed data - Los roles se sincronizan con la base de datos a través de migraciones
        SeedRoles(builder);
    }

    private static void SeedRoles(EntityTypeBuilder<Role> builder)
    {
        var roles = RoleConstants.GetAll();

        foreach (var role in roles)
        {
            builder.HasData(new
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description
            });

            // Seed del owned type AuditField
            builder.OwnsOne(r => r.AuditField).HasData(new
            {
                RoleId = role.Id,
                CreatedAt = role.AuditField.CreatedAt,
                UpdatedAt = role.AuditField.UpdatedAt,
                DeletedAt = role.AuditField.DeletedAt,
                IsActive = role.AuditField.IsActive
            });
        }
    }

}
