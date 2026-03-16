using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.AccountCatalogs.Create;

public record CreateAccountCatalogCommand(
    string AccountCode,
    string Name,
    int AccountType,
    int Nature,
    Guid? ParentId,
    int Level,
    bool AcceptsMovements) : IRequest<ErrorOr<AccountCatalogDto>>;
