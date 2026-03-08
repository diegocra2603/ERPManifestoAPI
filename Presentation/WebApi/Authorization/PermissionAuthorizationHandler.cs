using Application.Common.Constants;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Authorization;

/// <summary>
/// Handler que verifica si el usuario tiene el permiso requerido.
/// Los permisos se obtienen de los claims del token JWT.
/// </summary>
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Verificar si el usuario está autenticado
        if (context.User.Identity?.IsAuthenticated != true)
        {
            context.Fail(new AuthorizationFailureReason(this, "Usuario no autenticado"));
            return Task.CompletedTask;
        }

        // Obtener todos los permisos del usuario desde los claims
        var userPermissions = context.User
            .FindAll(CustomClaimTypes.Permission)
            .Select(c => c.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // SuperAdmin siempre tiene acceso: si tiene Admin.FullAccess puede acceder a cualquier ruta
        if (userPermissions.Contains(PermissionCodes.AdminFullAccess))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Verificar si el usuario tiene el permiso requerido
        if (userPermissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail(new AuthorizationFailureReason(
                this, 
                $"El usuario no tiene el permiso requerido: {requirement.Permission}"));
        }

        return Task.CompletedTask;
    }
}
