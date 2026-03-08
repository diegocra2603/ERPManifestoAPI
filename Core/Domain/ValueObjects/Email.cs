namespace Domain.ValueObjects;

public sealed record Email(string Value)
{
    public const int MaxLength = 256;

    /// <summary>
    /// Crea un Email válido. Retorna null si el valor es inválido.
    /// </summary>
    public static Email? Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        if (value.Length > MaxLength)
            return null;
        if (!value.Contains('@'))
            return null;

        return new Email(value.Trim().ToLowerInvariant());
    }
}
