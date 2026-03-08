namespace Domain.ValueObjects;

public sealed record PhoneNumber(string Value)
{
    public const int MinLength = 7;
    public const int MaxLength = 20;

    /// <summary>
    /// Crea un PhoneNumber válido. Retorna null si el valor es inválido.
    /// </summary>
    public static PhoneNumber? Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var normalized = new string(value.Where(c => char.IsDigit(c) || c == '+').ToArray());
        if (normalized.Length < MinLength || normalized.Length > MaxLength)
            return null;

        return new PhoneNumber(normalized);
    }

    public string Formatted => Value.Length >= 10
        ? $"+{Value[..1]} ({Value[1..4]}) {Value[4..7]}-{Value[7..]}"
        : Value;
}
