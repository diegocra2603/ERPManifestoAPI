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

    // Permisos de Product
    public const string ProductCreate = PermissionConstants.Codes.ProductCreate;
    public const string ProductRead = PermissionConstants.Codes.ProductRead;
    public const string ProductUpdate = PermissionConstants.Codes.ProductUpdate;
    public const string ProductDelete = PermissionConstants.Codes.ProductDelete;

    // Permisos de Category
    public const string CategoryCreate = PermissionConstants.Codes.CategoryCreate;
    public const string CategoryRead = PermissionConstants.Codes.CategoryRead;
    public const string CategoryUpdate = PermissionConstants.Codes.CategoryUpdate;
    public const string CategoryDelete = PermissionConstants.Codes.CategoryDelete;

    // Permisos de ProductOption
    public const string ProductOptionCreate = PermissionConstants.Codes.ProductOptionCreate;
    public const string ProductOptionRead = PermissionConstants.Codes.ProductOptionRead;
    public const string ProductOptionUpdate = PermissionConstants.Codes.ProductOptionUpdate;
    public const string ProductOptionDelete = PermissionConstants.Codes.ProductOptionDelete;

    // Permisos de Option
    public const string OptionCreate = PermissionConstants.Codes.OptionCreate;
    public const string OptionRead = PermissionConstants.Codes.OptionRead;
    public const string OptionUpdate = PermissionConstants.Codes.OptionUpdate;
    public const string OptionDelete = PermissionConstants.Codes.OptionDelete;

    // Permisos de Ingredient
    public const string IngredientCreate = PermissionConstants.Codes.IngredientCreate;
    public const string IngredientRead = PermissionConstants.Codes.IngredientRead;
    public const string IngredientUpdate = PermissionConstants.Codes.IngredientUpdate;
    public const string IngredientDelete = PermissionConstants.Codes.IngredientDelete;

    // Permisos de Store
    public const string StoreCreate = PermissionConstants.Codes.StoreCreate;
    public const string StoreRead = PermissionConstants.Codes.StoreRead;
    public const string StoreUpdate = PermissionConstants.Codes.StoreUpdate;
    public const string StoreDelete = PermissionConstants.Codes.StoreDelete;

    // Permisos de StoreArea
    public const string StoreAreaCreate = PermissionConstants.Codes.StoreAreaCreate;
    public const string StoreAreaRead = PermissionConstants.Codes.StoreAreaRead;
    public const string StoreAreaUpdate = PermissionConstants.Codes.StoreAreaUpdate;
    public const string StoreAreaDelete = PermissionConstants.Codes.StoreAreaDelete;

    // Permisos de Table
    public const string TableCreate = PermissionConstants.Codes.TableCreate;
    public const string TableRead = PermissionConstants.Codes.TableRead;
    public const string TableUpdate = PermissionConstants.Codes.TableUpdate;
    public const string TableDelete = PermissionConstants.Codes.TableDelete;

    // Permisos de Check
    public const string CheckCreate = PermissionConstants.Codes.CheckCreate;
    public const string CheckRead = PermissionConstants.Codes.CheckRead;
    public const string CheckUpdate = PermissionConstants.Codes.CheckUpdate;
    public const string CheckDelete = PermissionConstants.Codes.CheckDelete;

    // Permisos de CheckPayment
    public const string CheckPaymentCreate = PermissionConstants.Codes.CheckPaymentCreate;

    // Permisos de DayOpening
    public const string DayOpeningCreate = PermissionConstants.Codes.DayOpeningCreate;
    public const string DayOpeningRead = PermissionConstants.Codes.DayOpeningRead;

    // Permisos de DayClosing
    public const string DayClosingCreate = PermissionConstants.Codes.DayClosingCreate;
    public const string DayClosingRead = PermissionConstants.Codes.DayClosingRead;

    // Permisos de Administración
    public const string AdminFullAccess = PermissionConstants.Codes.AdminFullAccess;
}
