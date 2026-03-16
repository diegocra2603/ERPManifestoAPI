using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Suppliers.Delete;

public record DeleteSupplierCommand(Guid Id) : IRequest<ErrorOr<bool>>;
