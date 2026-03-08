using Microsoft.AspNetCore.Authorization;

namespace WebApi.Authorization;

/// <summary>
/// Atributo para autorización basada en permisos.
/// Uso: [HasPermission(PermissionCodes.UserCreate)]
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission) : base(permission)
    {
    }
}
