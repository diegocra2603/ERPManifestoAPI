using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Suppliers.Update;

public record UpdateSupplierCommand(
    Guid Id,
    string Nit,
    string Name,
    string? Address,
    string? Phone,
    string? Email) : IRequest<ErrorOr<SupplierDto>>;
