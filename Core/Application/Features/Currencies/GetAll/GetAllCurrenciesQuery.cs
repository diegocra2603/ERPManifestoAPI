using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Currencies.GetAll;

public record GetAllCurrenciesQuery : IRequest<ErrorOr<IReadOnlyList<CurrencyDto>>>;
