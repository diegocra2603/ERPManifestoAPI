using Domain.Entities.Permissions;
using Domain.Entities.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

        builder.Property(rp => rp.RoleId)
            .HasConversion(id => id.Value, v => new RoleId(v));

        builder.Property(rp => rp.PermissionId)
            .HasConversion(id => id.Value, v => new PermissionId(v));

        SeedRolePermissions(builder);
    }

    private static void SeedRolePermissions(EntityTypeBuilder<RolePermission> builder)
    {
        var rolePermissions = RoleConstants.GetRolePermissions();

        foreach (var (roleId, permissionId) in rolePermissions)
        {
            builder.HasData(new RolePermission(
                new RoleId(roleId),
                new PermissionId(permissionId)));
        }
    }
}
