using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Currencies.Delete;

public record DeleteCurrencyCommand(Guid Id) : IRequest<ErrorOr<bool>>;
