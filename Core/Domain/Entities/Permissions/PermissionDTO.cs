namespace Domain.Entities.Permissions;

public record PermissionDto(
    Guid Id,
    string Name,
    string Description,
    string Code
);