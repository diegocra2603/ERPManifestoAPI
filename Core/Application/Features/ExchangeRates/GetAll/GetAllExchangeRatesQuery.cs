using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.ExchangeRates.GetAll;

public record GetAllExchangeRatesQuery : IRequest<ErrorOr<IReadOnlyList<ExchangeRateDto>>>;
