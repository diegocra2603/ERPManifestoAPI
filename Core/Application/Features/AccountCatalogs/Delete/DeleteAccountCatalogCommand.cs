using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.AccountCatalogs.Delete;

public record DeleteAccountCatalogCommand(Guid Id) : IRequest<ErrorOr<bool>>;
