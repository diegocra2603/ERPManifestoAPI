using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.ExchangeRates.Delete;

public record DeleteExchangeRateCommand(Guid Id) : IRequest<ErrorOr<bool>>;
