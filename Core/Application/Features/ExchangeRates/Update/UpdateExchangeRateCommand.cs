using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.ExchangeRates.Update;

public record UpdateExchangeRateCommand(
    Guid Id,
    decimal BuyRate,
    decimal SellRate,
    string Source) : IRequest<ErrorOr<ExchangeRateDto>>;
