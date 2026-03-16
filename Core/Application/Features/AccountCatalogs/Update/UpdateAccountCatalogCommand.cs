using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.AccountCatalogs.Update;

public record UpdateAccountCatalogCommand(
    Guid Id,
    string AccountCode,
    string Name,
    int AccountType,
    int Nature,
    Guid? ParentId,
    int Level,
    bool AcceptsMovements) : IRequest<ErrorOr<AccountCatalogDto>>;
