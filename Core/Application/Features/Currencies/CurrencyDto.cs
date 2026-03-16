namespace Application.Features.Currencies;

public record CurrencyDto(
    Guid Id,
    string Code,
    string Name,
    string Symbol,
    bool IsFunctional,
    int DecimalPlaces
);
