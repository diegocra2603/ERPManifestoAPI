using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Currencies.Create;

public record CreateCurrencyCommand(
    string Code,
    string Name,
    string Symbol,
    bool IsFunctional,
    int DecimalPlaces) : IRequest<ErrorOr<CurrencyDto>>;
