using System.Security.Cryptography;

namespace Application.Helpers;

/// <summary>
/// Helper para generación de contraseñas temporales.
/// </summary>
public static class PasswordHelper
{
    private const int MinValue = 100_000; // 6 dígitos mínimo
    private const int MaxValue = 1_000_000; // exclusivo: 999999 máximo

    /// <summary>
    /// Genera una contraseña temporal de 6 dígitos numéricos.
    /// </summary>
    /// <returns>Una cadena de 6 dígitos (100000-999999).</returns>
    public static string GenerateTemporalPassword()
    {
        return RandomNumberGenerator.GetInt32(MinValue, MaxValue).ToString();
    }
}
