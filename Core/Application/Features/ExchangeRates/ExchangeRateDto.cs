namespace Application.Features.ExchangeRates;

public record ExchangeRateDto(
    Guid Id,
    Guid CurrencyId,
    string CurrencyCode,
    string CurrencyName,
    DateTime Date,
    decimal BuyRate,
    decimal SellRate,
    string Source
);
