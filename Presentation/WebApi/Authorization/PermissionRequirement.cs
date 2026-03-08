using Microsoft.AspNetCore.Authorization;

namespace WebApi.Authorization;

/// <summary>
/// Requirement que especifica qué permiso se necesita para acceder a un recurso.
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}
