using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace WebApi.Authorization;

/// <summary>
/// Policy provider que crea políticas de autorización dinámicamente basadas en permisos.
/// Cada permiso se convierte en una política con el mismo nombre.
/// </summary>
public class PermissionAuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

    public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // Si el policyName contiene un punto, asumimos que es un permiso (ej: "User.Create")
        if (policyName.Contains('.'))
        {
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();

            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        // Si no es un permiso, usar el provider por defecto
        return _fallbackPolicyProvider.GetPolicyAsync(policyName);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
        _fallbackPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
        _fallbackPolicyProvider.GetFallbackPolicyAsync();
}
