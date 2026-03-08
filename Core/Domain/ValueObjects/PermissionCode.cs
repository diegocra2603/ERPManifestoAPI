namespace Domain.ValueObjects;

/// <summary>
/// Código de permiso. Formato recomendado: "Recurso.Accion" (ej. "Users.Read", "Roles.Write").
/// </summary>
public sealed record PermissionCode(string Value)
{
    public const int MaxLength = 64;
    public const int MinLength = 1;

    /// <summary>
    /// Crea un PermissionCode válido. Retorna null si el valor es inválido.
    /// </summary>
    public static PermissionCode? Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        if (value.Length < MinLength || value.Length > MaxLength)
            return null;

        return new PermissionCode(value.Trim());
    }
}
