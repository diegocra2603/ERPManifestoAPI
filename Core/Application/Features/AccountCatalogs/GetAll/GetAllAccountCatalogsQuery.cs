using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.AccountCatalogs.GetAll;

public record GetAllAccountCatalogsQuery : IRequest<ErrorOr<IReadOnlyList<AccountCatalogDto>>>;
