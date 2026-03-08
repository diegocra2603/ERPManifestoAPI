using Domain.Entities.Permissions;

namespace Application.Common.Constants;

/// <summary>
/// Códigos de permisos disponibles en el sistema.
/// Esta clase expone los códigos definidos en el dominio para uso en la capa de aplicación.
/// </summary>
public static class PermissionCodes
{
    // Permisos de Usuario
    public const string UserCreate = PermissionConstants.Codes.UserCreate;
    public const string UserRead = PermissionConstants.Codes.UserRead;
    public const string UserUpdate = PermissionConstants.Codes.UserUpdate;
    public const string UserDelete = PermissionConstants.Codes.UserDelete;

    // Permisos de Rol
    public const string RoleCreate = PermissionConstants.Codes.RoleCreate;
    public const string RoleRead = PermissionConstants.Codes.RoleRead;
    public const string RoleUpdate = PermissionConstants.Codes.RoleUpdate;
    public const string RoleDelete = PermissionConstants.Codes.RoleDelete;

    // Permisos de JobPosition
    public const string JobPositionCreate = PermissionConstants.Codes.JobPositionCreate;
    public const string JobPositionRead = PermissionConstants.Codes.JobPositionRead;
    public const string JobPositionUpdate = PermissionConstants.Codes.JobPositionUpdate;
    public const string JobPositionDelete = PermissionConstants.Codes.JobPositionDelete;

    // Permisos de Administración
    public const string AdminFullAccess = PermissionConstants.Codes.AdminFullAccess;
}
