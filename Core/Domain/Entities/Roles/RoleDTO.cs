using Domain.Entities.Permissions;

namespace Domain.Entities.Roles;

public record RoleDto(
    Guid Id,
    string Name,
    string Description,
    IReadOnlyList<PermissionDto>? Permissions = null
);