using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Suppliers.GetAll;

public record GetAllSuppliersQuery : IRequest<ErrorOr<IReadOnlyList<SupplierDto>>>;
