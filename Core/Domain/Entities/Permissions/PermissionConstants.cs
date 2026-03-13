using Domain.ValueObjects;

namespace Domain.Entities.Permissions;

/// <summary>
/// Constantes de permisos del sistema. Estos valores son inmutables y se sincronizan
/// con la base de datos a través de migraciones de EF Core.
/// </summary>
public static class PermissionConstants
{
    /// <summary>
    /// IDs fijos para cada permiso. NUNCA cambiar estos valores una vez en producción.
    /// </summary>
    public static class Ids
    {
        // Permisos de Usuario
        public static readonly Guid UserCreate = new("a5037d95-d672-4bdf-80ea-c22bc687e85f");
        public static readonly Guid UserRead = new("4604fa93-c900-4b6e-aad2-1f424a4d3f01");
        public static readonly Guid UserUpdate = new("d14e36b8-f5b7-454b-91dd-714901ce54d2");
        public static readonly Guid UserDelete = new("17c86b19-c4b0-4596-a016-cb568a343367");

        // Permisos de Rol
        public static readonly Guid RoleCreate = new("87c67f54-568a-4cbf-8992-05b4cfa20b0c");
        public static readonly Guid RoleRead = new("1d8c3417-2833-42e2-9338-2ed2ba5a3290");
        public static readonly Guid RoleUpdate = new("283a2cc6-3e80-41ca-90f7-227f7bcefe4c");
        public static readonly Guid RoleDelete = new("8e958e23-2110-4798-b6ac-f04dd0dd156d");

        // Permisos de JobPosition
        public static readonly Guid JobPositionCreate = new("a1d2e3f4-b5c6-4a7b-8d9e-0f1a2b3c4d5e");
        public static readonly Guid JobPositionRead = new("b2e3f4a5-c6d7-4b8c-9e0f-1a2b3c4d5e6f");
        public static readonly Guid JobPositionUpdate = new("c3f4a5b6-d7e8-4c9d-0f1a-2b3c4d5e6f7a");
        public static readonly Guid JobPositionDelete = new("d4a5b6c7-e8f9-4d0e-1a2b-3c4d5e6f7a8b");

        // Permisos de SystemSetting
        public static readonly Guid SettingRead = new("e5b6c7d8-f9a0-4e1f-2b3c-4d5e6f7a8b9c");
        public static readonly Guid SettingUpdate = new("f6c7d8e9-a0b1-4f2a-3c4d-5e6f7a8b9c0d");

        // Permisos de Administración
        public static readonly Guid AdminFullAccess = new("f585a827-07a3-435f-89a0-b6734842ffea");
    }

    /// <summary>
    /// Códigos de permisos para usar en atributos de autorización.
    /// </summary>
    public static class Codes
    {
        // Permisos de Usuario
        public const string UserCreate = "User.Create";
        public const string UserRead = "User.Read";
        public const string UserUpdate = "User.Update";
        public const string UserDelete = "User.Delete";

        // Permisos de Rol
        public const string RoleCreate = "Role.Create";
        public const string RoleRead = "Role.Read";
        public const string RoleUpdate = "Role.Update";
        public const string RoleDelete = "Role.Delete";

        // Permisos de JobPosition
        public const string JobPositionCreate = "JobPosition.Create";
        public const string JobPositionRead = "JobPosition.Read";
        public const string JobPositionUpdate = "JobPosition.Update";
        public const string JobPositionDelete = "JobPosition.Delete";

        // Permisos de SystemSetting
        public const string SettingRead = "Setting.Read";
        public const string SettingUpdate = "Setting.Update";

        // Permisos de Administración
        public const string AdminFullAccess = "Admin.FullAccess";
    }

    /// <summary>
    /// Fecha de creación por defecto para seed data.
    /// </summary>
    private static readonly DateTime SeedCreatedAt = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Obtiene todos los permisos del sistema para seed data.
    /// </summary>
    public static IReadOnlyList<Permission> GetAll() =>
    [
        // Permisos de Usuario
        CreatePermission(Ids.UserCreate, "Crear Usuario", "Permite crear nuevos usuarios en el sistema", Codes.UserCreate),
        CreatePermission(Ids.UserRead, "Ver Usuario", "Permite ver información de usuarios", Codes.UserRead),
        CreatePermission(Ids.UserUpdate, "Actualizar Usuario", "Permite actualizar información de usuarios", Codes.UserUpdate),
        CreatePermission(Ids.UserDelete, "Eliminar Usuario", "Permite eliminar usuarios del sistema", Codes.UserDelete),

        // Permisos de Rol
        CreatePermission(Ids.RoleCreate, "Crear Rol", "Permite crear nuevos roles en el sistema", Codes.RoleCreate),
        CreatePermission(Ids.RoleRead, "Ver Rol", "Permite ver información de roles", Codes.RoleRead),
        CreatePermission(Ids.RoleUpdate, "Actualizar Rol", "Permite actualizar información de roles", Codes.RoleUpdate),
        CreatePermission(Ids.RoleDelete, "Eliminar Rol", "Permite eliminar roles del sistema", Codes.RoleDelete),

        // Permisos de JobPosition
        CreatePermission(Ids.JobPositionCreate, "Crear Puesto de Trabajo", "Permite crear nuevos puestos de trabajo", Codes.JobPositionCreate),
        CreatePermission(Ids.JobPositionRead, "Ver Puesto de Trabajo", "Permite ver información de puestos de trabajo", Codes.JobPositionRead),
        CreatePermission(Ids.JobPositionUpdate, "Actualizar Puesto de Trabajo", "Permite actualizar puestos de trabajo", Codes.JobPositionUpdate),
        CreatePermission(Ids.JobPositionDelete, "Eliminar Puesto de Trabajo", "Permite eliminar puestos de trabajo", Codes.JobPositionDelete),

        // Permisos de SystemSetting
        CreatePermission(Ids.SettingRead, "Ver Configuración", "Permite ver configuraciones del sistema", Codes.SettingRead),
        CreatePermission(Ids.SettingUpdate, "Actualizar Configuración", "Permite actualizar configuraciones del sistema", Codes.SettingUpdate),

        // Permisos de Administración
        CreatePermission(Ids.AdminFullAccess, "Acceso Total", "Acceso completo a todas las funcionalidades del sistema", Codes.AdminFullAccess),
    ];

    private static Permission CreatePermission(Guid id, string name, string description, string code) =>
        new(
            new PermissionId(id),
            name,
            description,
            code,
            new AuditField(SeedCreatedAt, null, null, true)
        );
}
