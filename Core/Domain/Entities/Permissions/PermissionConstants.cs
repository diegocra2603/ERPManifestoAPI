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

        // Permisos de Product
        public static readonly Guid ProductCreate = new("b8f1a9d4-e5c2-4a7b-9f3e-1d6c8a4b2e5f");
        public static readonly Guid ProductRead = new("c9e2b8f5-d6a3-4b8c-a0f4-2e7d9b5c3f6a");
        public static readonly Guid ProductUpdate = new("d0f3c9a6-e7b4-4c9d-b1e5-3f8e0c6d4a7b");
        public static readonly Guid ProductDelete = new("e1a4d0b7-f8c5-4d0e-c2f6-4a9f1d7e5b8c");

        // Permisos de Category
        public static readonly Guid CategoryCreate = new("f2b5e1c8-a9d6-4e3f-b7a0-5c8d1e9f2a6b");
        public static readonly Guid CategoryRead = new("a3c6f2d9-b0e7-4f8a-c1b4-6d9e2f0a3b7c");
        public static readonly Guid CategoryUpdate = new("b4d7a3e0-c1f8-4a9b-d2c5-7e0f3a1b4c8d");
        public static readonly Guid CategoryDelete = new("c5e8b4f1-d2a9-4b0c-e3d6-8f1a4b2c5d9e");

        // Permisos de ProductOption
        public static readonly Guid ProductOptionCreate = new("b0d3a9e6-c7f4-4a5b-d8c1-3e6f9a7b0c4d");
        public static readonly Guid ProductOptionRead = new("c1e4b0f7-d8a5-4b6c-e9d2-4f7a0b8c1d5e");
        public static readonly Guid ProductOptionUpdate = new("d2f5c1a8-e9b6-4c7d-f0e3-5a8b1c9d2e6f");
        public static readonly Guid ProductOptionDelete = new("e3a6d2b9-f0c7-4d8e-a1f4-6b9c2d0e3f7a");

        // Permisos de Option
        public static readonly Guid OptionCreate = new("d8f1a4b5-e2c9-4d3e-f6a7-1b0c5d8e3f9a");
        public static readonly Guid OptionRead = new("e9a2b5c6-f3d0-4e4f-a7b8-2c1d6e9f4a0b");
        public static readonly Guid OptionUpdate = new("f0b3c6d7-a4e1-4f5a-b8c9-3d2e7f0a5b1c");
        public static readonly Guid OptionDelete = new("a1c4d7e8-b5f2-4a6b-c9d0-4e3f8a1b6c2d");

        // Permisos de Ingredient
        public static readonly Guid IngredientCreate = new("f4b7e3c0-a1d8-4e9f-b2a5-7c0d3e1f4a8b");
        public static readonly Guid IngredientRead = new("a5c8f4d1-b2e9-4f0a-c3b6-8d1e4f2a5b9c");
        public static readonly Guid IngredientUpdate = new("b6d9a5e2-c3f0-4a1b-d4c7-9e2f5a3b6c0d");
        public static readonly Guid IngredientDelete = new("c7e0b6f3-d4a1-4b2c-e5d8-0f3a6b4c7d1e");

        // Permisos de Store
        public static readonly Guid StoreCreate = new("a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d");
        public static readonly Guid StoreRead = new("b2c3d4e5-f6a7-4b8c-9d0e-1f2a3b4c5d6e");
        public static readonly Guid StoreUpdate = new("c3d4e5f6-a7b8-4c9d-0e1f-2a3b4c5d6e7f");
        public static readonly Guid StoreDelete = new("d4e5f6a7-b8c9-4d0e-1f2a-3b4c5d6e7f8a");

        // Permisos de StoreArea
        public static readonly Guid StoreAreaCreate = new("e5f6a7b8-c9d0-4e1f-2a3b-4c5d6e7f8a9b");
        public static readonly Guid StoreAreaRead = new("f6a7b8c9-d0e1-4f2a-3b4c-5d6e7f8a9b0c");
        public static readonly Guid StoreAreaUpdate = new("a7b8c9d0-e1f2-4a3b-4c5d-6e7f8a9b0c1d");
        public static readonly Guid StoreAreaDelete = new("b8c9d0e1-f2a3-4b4c-5d6e-7f8a9b0c1d2e");

        // Permisos de Table
        public static readonly Guid TableCreate = new("c9d0e1f2-a3b4-4c5d-6e7f-8a9b0c1d2e3f");
        public static readonly Guid TableRead = new("d0e1f2a3-b4c5-4d6e-7f8a-9b0c1d2e3f4a");
        public static readonly Guid TableUpdate = new("e1f2a3b4-c5d6-4e7f-8a9b-0c1d2e3f4a5b");
        public static readonly Guid TableDelete = new("f2a3b4c5-d6e7-4f8a-9b0c-1d2e3f4a5b6c");

        // Permisos de Check
        public static readonly Guid CheckCreate = new("d1a2b3c4-e5f6-4a7b-8c9d-0e1f2a3b4c5d");
        public static readonly Guid CheckRead = new("e2b3c4d5-f6a7-4b8c-9d0e-1f2a3b4c5d6e");
        public static readonly Guid CheckUpdate = new("f3c4d5e6-a7b8-4c9d-0e1f-2a3b4c5d6e7f");
        public static readonly Guid CheckDelete = new("a4d5e6f7-b8c9-4d0e-1f2a-3b4c5d6e7f8a");

        // Permisos de CheckPayment
        public static readonly Guid CheckPaymentCreate = new("b5e6f7a8-c9d0-4e1f-2a3b-4c5d6e7f8a9b");

        // Permisos de DayOpening
        public static readonly Guid DayOpeningCreate = new("c6f7a8b9-d0e1-4f2a-3b4c-5d6e7f8a9b0c");
        public static readonly Guid DayOpeningRead = new("d7a8b9c0-e1f2-4a3b-4c5d-6e7f8a9b0c1d");

        // Permisos de DayClosing
        public static readonly Guid DayClosingCreate = new("e8b9c0d1-f2a3-4b4c-5d6e-7f8a9b0c1d2e");
        public static readonly Guid DayClosingRead = new("f9c0d1e2-a3b4-4c5d-6e7f-8a9b0c1d2e3f");

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

        // Permisos de Product
        public const string ProductCreate = "Product.Create";
        public const string ProductRead = "Product.Read";
        public const string ProductUpdate = "Product.Update";
        public const string ProductDelete = "Product.Delete";

        // Permisos de Category
        public const string CategoryCreate = "Category.Create";
        public const string CategoryRead = "Category.Read";
        public const string CategoryUpdate = "Category.Update";
        public const string CategoryDelete = "Category.Delete";

        // Permisos de ProductOption
        public const string ProductOptionCreate = "ProductOption.Create";
        public const string ProductOptionRead = "ProductOption.Read";
        public const string ProductOptionUpdate = "ProductOption.Update";
        public const string ProductOptionDelete = "ProductOption.Delete";

        // Permisos de Option
        public const string OptionCreate = "Option.Create";
        public const string OptionRead = "Option.Read";
        public const string OptionUpdate = "Option.Update";
        public const string OptionDelete = "Option.Delete";

        // Permisos de Ingredient
        public const string IngredientCreate = "Ingredient.Create";
        public const string IngredientRead = "Ingredient.Read";
        public const string IngredientUpdate = "Ingredient.Update";
        public const string IngredientDelete = "Ingredient.Delete";

        // Permisos de Store
        public const string StoreCreate = "Store.Create";
        public const string StoreRead = "Store.Read";
        public const string StoreUpdate = "Store.Update";
        public const string StoreDelete = "Store.Delete";

        // Permisos de StoreArea
        public const string StoreAreaCreate = "StoreArea.Create";
        public const string StoreAreaRead = "StoreArea.Read";
        public const string StoreAreaUpdate = "StoreArea.Update";
        public const string StoreAreaDelete = "StoreArea.Delete";

        // Permisos de Table
        public const string TableCreate = "Table.Create";
        public const string TableRead = "Table.Read";
        public const string TableUpdate = "Table.Update";
        public const string TableDelete = "Table.Delete";

        // Permisos de Check
        public const string CheckCreate = "Check.Create";
        public const string CheckRead = "Check.Read";
        public const string CheckUpdate = "Check.Update";
        public const string CheckDelete = "Check.Delete";

        // Permisos de CheckPayment
        public const string CheckPaymentCreate = "CheckPayment.Create";

        // Permisos de DayOpening
        public const string DayOpeningCreate = "DayOpening.Create";
        public const string DayOpeningRead = "DayOpening.Read";

        // Permisos de DayClosing
        public const string DayClosingCreate = "DayClosing.Create";
        public const string DayClosingRead = "DayClosing.Read";

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

        // Permisos de Product
        CreatePermission(Ids.ProductCreate, "Crear Producto", "Permite crear nuevos productos en el sistema", Codes.ProductCreate),
        CreatePermission(Ids.ProductRead, "Ver Producto", "Permite ver información de productos", Codes.ProductRead),
        CreatePermission(Ids.ProductUpdate, "Actualizar Producto", "Permite actualizar información de productos", Codes.ProductUpdate),
        CreatePermission(Ids.ProductDelete, "Eliminar Producto", "Permite eliminar productos del sistema", Codes.ProductDelete),

        // Permisos de Category
        CreatePermission(Ids.CategoryCreate, "Crear Categoría", "Permite crear nuevas categorías en el sistema", Codes.CategoryCreate),
        CreatePermission(Ids.CategoryRead, "Ver Categoría", "Permite ver información de categorías", Codes.CategoryRead),
        CreatePermission(Ids.CategoryUpdate, "Actualizar Categoría", "Permite actualizar información de categorías", Codes.CategoryUpdate),
        CreatePermission(Ids.CategoryDelete, "Eliminar Categoría", "Permite eliminar categorías del sistema", Codes.CategoryDelete),

        // Permisos de ProductOption
        CreatePermission(Ids.ProductOptionCreate, "Crear Opción de Producto", "Permite crear nuevas opciones de producto", Codes.ProductOptionCreate),
        CreatePermission(Ids.ProductOptionRead, "Ver Opción de Producto", "Permite ver información de opciones de producto", Codes.ProductOptionRead),
        CreatePermission(Ids.ProductOptionUpdate, "Actualizar Opción de Producto", "Permite actualizar opciones de producto", Codes.ProductOptionUpdate),
        CreatePermission(Ids.ProductOptionDelete, "Eliminar Opción de Producto", "Permite eliminar opciones de producto", Codes.ProductOptionDelete),

        // Permisos de Option
        CreatePermission(Ids.OptionCreate, "Crear Opción", "Permite crear nuevas opciones en el sistema", Codes.OptionCreate),
        CreatePermission(Ids.OptionRead, "Ver Opción", "Permite ver información de opciones", Codes.OptionRead),
        CreatePermission(Ids.OptionUpdate, "Actualizar Opción", "Permite actualizar opciones del sistema", Codes.OptionUpdate),
        CreatePermission(Ids.OptionDelete, "Eliminar Opción", "Permite eliminar opciones del sistema", Codes.OptionDelete),

        // Permisos de Ingredient
        CreatePermission(Ids.IngredientCreate, "Crear Ingrediente", "Permite crear nuevos ingredientes en el sistema", Codes.IngredientCreate),
        CreatePermission(Ids.IngredientRead, "Ver Ingrediente", "Permite ver información de ingredientes", Codes.IngredientRead),
        CreatePermission(Ids.IngredientUpdate, "Actualizar Ingrediente", "Permite actualizar información de ingredientes", Codes.IngredientUpdate),
        CreatePermission(Ids.IngredientDelete, "Eliminar Ingrediente", "Permite eliminar ingredientes del sistema", Codes.IngredientDelete),

        // Permisos de Store
        CreatePermission(Ids.StoreCreate, "Crear Tienda", "Permite crear nuevas tiendas en el sistema", Codes.StoreCreate),
        CreatePermission(Ids.StoreRead, "Ver Tienda", "Permite ver información de tiendas", Codes.StoreRead),
        CreatePermission(Ids.StoreUpdate, "Actualizar Tienda", "Permite actualizar información de tiendas", Codes.StoreUpdate),
        CreatePermission(Ids.StoreDelete, "Eliminar Tienda", "Permite eliminar tiendas del sistema", Codes.StoreDelete),

        // Permisos de StoreArea
        CreatePermission(Ids.StoreAreaCreate, "Crear Área de Tienda", "Permite crear nuevas áreas de tienda", Codes.StoreAreaCreate),
        CreatePermission(Ids.StoreAreaRead, "Ver Área de Tienda", "Permite ver información de áreas de tienda", Codes.StoreAreaRead),
        CreatePermission(Ids.StoreAreaUpdate, "Actualizar Área de Tienda", "Permite actualizar áreas de tienda", Codes.StoreAreaUpdate),
        CreatePermission(Ids.StoreAreaDelete, "Eliminar Área de Tienda", "Permite eliminar áreas de tienda", Codes.StoreAreaDelete),

        // Permisos de Table
        CreatePermission(Ids.TableCreate, "Crear Mesa", "Permite crear nuevas mesas en el sistema", Codes.TableCreate),
        CreatePermission(Ids.TableRead, "Ver Mesa", "Permite ver información de mesas", Codes.TableRead),
        CreatePermission(Ids.TableUpdate, "Actualizar Mesa", "Permite actualizar información de mesas", Codes.TableUpdate),
        CreatePermission(Ids.TableDelete, "Eliminar Mesa", "Permite eliminar mesas del sistema", Codes.TableDelete),

        // Permisos de Check
        CreatePermission(Ids.CheckCreate, "Crear Cuenta", "Permite crear nuevas cuentas en el sistema", Codes.CheckCreate),
        CreatePermission(Ids.CheckRead, "Ver Cuenta", "Permite ver información de cuentas", Codes.CheckRead),
        CreatePermission(Ids.CheckUpdate, "Actualizar Cuenta", "Permite actualizar información de cuentas", Codes.CheckUpdate),
        CreatePermission(Ids.CheckDelete, "Eliminar Cuenta", "Permite cancelar cuentas del sistema", Codes.CheckDelete),

        // Permisos de CheckPayment
        CreatePermission(Ids.CheckPaymentCreate, "Crear Pago", "Permite registrar pagos en cuentas", Codes.CheckPaymentCreate),

        // Permisos de DayOpening
        CreatePermission(Ids.DayOpeningCreate, "Apertura de Día", "Permite registrar aperturas de día", Codes.DayOpeningCreate),
        CreatePermission(Ids.DayOpeningRead, "Ver Apertura de Día", "Permite ver aperturas de día", Codes.DayOpeningRead),

        // Permisos de DayClosing
        CreatePermission(Ids.DayClosingCreate, "Cierre de Día", "Permite registrar cierres de día", Codes.DayClosingCreate),
        CreatePermission(Ids.DayClosingRead, "Ver Cierre de Día", "Permite ver cierres de día", Codes.DayClosingRead),

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
