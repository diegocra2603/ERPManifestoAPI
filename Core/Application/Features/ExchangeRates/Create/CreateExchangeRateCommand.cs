using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.ExchangeRates.Create;

public record CreateExchangeRateCommand(
    Guid CurrencyId,
    DateTime Date,
    decimal BuyRate,
    decimal SellRate,
    string Source) : IRequest<ErrorOr<ExchangeRateDto>>;
