using Domain.Entities.Permissions;
using Domain.ValueObjects;

namespace Domain.Entities.Roles;

/// <summary>
/// Constantes de roles del sistema. Estos valores son inmutables y se sincronizan
/// con la base de datos a través de migraciones de EF Core.
/// </summary>
public static class RoleConstants
{
    /// <summary>
    /// IDs fijos para cada rol. NUNCA cambiar estos valores una vez en producción.
    /// </summary>
    public static class Ids
    {
        public static readonly Guid SuperAdmin = new("3f039871-c926-4313-b79c-8f17e622ec59");
        public static readonly Guid Admin = new("1159c308-279e-4bba-bae2-e8063c6d908e");
        public static readonly Guid User = new("f8109383-afb7-47a4-b97e-2f4c67c11d54");
    }

    /// <summary>
    /// Nombres de roles para usar en validaciones.
    /// </summary>
    public static class Names
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        public const string User = "User";  
    }

    /// <summary>
    /// Fecha de creación por defecto para seed data.
    /// </summary>
    private static readonly DateTime SeedCreatedAt = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Obtiene todos los roles del sistema para seed data.
    /// </summary>
    public static IReadOnlyList<Role> GetAll() =>
    [
        CreateRole(Ids.SuperAdmin, Names.SuperAdmin, "Rol con acceso total al sistema"),
        CreateRole(Ids.Admin, Names.Admin, "Rol de administrador con acceso a gestión de usuarios y roles"),
        CreateRole(Ids.User, Names.User, "Rol básico de usuario con permisos limitados"),
    ];

    /// <summary>
    /// Define qué permisos tiene cada rol.
    /// Retorna una lista de tuplas (RoleId, PermissionId) para la tabla intermedia.
    /// </summary>
    public static IReadOnlyList<(Guid RoleId, Guid PermissionId)> GetRolePermissions() =>
    [
        // SuperAdmin - Todos los permisos
        (Ids.SuperAdmin, PermissionConstants.Ids.AdminFullAccess),
        (Ids.SuperAdmin, PermissionConstants.Ids.UserCreate),
        (Ids.SuperAdmin, PermissionConstants.Ids.UserRead),
        (Ids.SuperAdmin, PermissionConstants.Ids.UserUpdate),
        (Ids.SuperAdmin, PermissionConstants.Ids.UserDelete),
        (Ids.SuperAdmin, PermissionConstants.Ids.RoleCreate),
        (Ids.SuperAdmin, PermissionConstants.Ids.RoleRead),
        (Ids.SuperAdmin, PermissionConstants.Ids.RoleUpdate),
        (Ids.SuperAdmin, PermissionConstants.Ids.RoleDelete),

        // SuperAdmin - DayOperation permissions
        (Ids.SuperAdmin, PermissionConstants.Ids.DayOpeningCreate),
        (Ids.SuperAdmin, PermissionConstants.Ids.DayOpeningRead),
        (Ids.SuperAdmin, PermissionConstants.Ids.DayClosingCreate),
        (Ids.SuperAdmin, PermissionConstants.Ids.DayClosingRead),

        // SuperAdmin - Check permissions
        (Ids.SuperAdmin, PermissionConstants.Ids.CheckCreate),
        (Ids.SuperAdmin, PermissionConstants.Ids.CheckRead),
        (Ids.SuperAdmin, PermissionConstants.Ids.CheckUpdate),
        (Ids.SuperAdmin, PermissionConstants.Ids.CheckDelete),
        (Ids.SuperAdmin, PermissionConstants.Ids.CheckPaymentCreate),

        // Admin - Gestión de usuarios y roles (sin eliminar)
        (Ids.Admin, PermissionConstants.Ids.UserCreate),
        (Ids.Admin, PermissionConstants.Ids.UserRead),
        (Ids.Admin, PermissionConstants.Ids.UserUpdate),
        (Ids.Admin, PermissionConstants.Ids.RoleRead),
        (Ids.Admin, PermissionConstants.Ids.DayOpeningCreate),
        (Ids.Admin, PermissionConstants.Ids.DayOpeningRead),
        (Ids.Admin, PermissionConstants.Ids.DayClosingCreate),
        (Ids.Admin, PermissionConstants.Ids.DayClosingRead),
        (Ids.Admin, PermissionConstants.Ids.CheckCreate),
        (Ids.Admin, PermissionConstants.Ids.CheckRead),
        (Ids.Admin, PermissionConstants.Ids.CheckUpdate),
        (Ids.Admin, PermissionConstants.Ids.CheckDelete),
        (Ids.Admin, PermissionConstants.Ids.CheckPaymentCreate),

        // User - Solo lectura básica
        (Ids.User, PermissionConstants.Ids.UserRead),

    ];

    private static Role CreateRole(Guid id, string name, string description) =>
        new(
            new RoleId(id),
            name,
            description,
            new AuditField(SeedCreatedAt, null, null, true)
        );
}
