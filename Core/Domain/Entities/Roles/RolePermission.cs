using Domain.Entities.Permissions;

namespace Domain.Entities.Roles;

/// <summary>
/// Entidad de unión entre Role y Permission (tabla RolePermissions).
/// </summary>
public sealed class RolePermission
{
    private RolePermission() { }

    public RolePermission(RoleId roleId, PermissionId permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }

    public RoleId RoleId { get; private set; } = default!;
    public PermissionId PermissionId { get; private set; } = default!;

    public Role Role { get; private set; } = default!;
    public Permission Permission { get; private set; } = default!;
}
