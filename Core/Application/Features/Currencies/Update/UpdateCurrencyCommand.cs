using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Currencies.Update;

public record UpdateCurrencyCommand(
    Guid Id,
    string Code,
    string Name,
    string Symbol,
    bool IsFunctional,
    int DecimalPlaces) : IRequest<ErrorOr<CurrencyDto>>;
