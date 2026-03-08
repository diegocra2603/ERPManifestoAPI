namespace Domain.ValueObjects;

public sealed record Name(string Value)
{
    public const int MinLength = 1;
    public const int MaxLength = 256;

    /// <summary>
    /// Crea un Name válido. Retorna null si el valor es inválido.
    /// </summary>
    public static Name? Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        if (value.Length < MinLength || value.Length > MaxLength)
            return null;

        return new Name(value.Trim());
    }
}
