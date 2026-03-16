using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Suppliers.Create;

public record CreateSupplierCommand(
    string Nit,
    string Name,
    string? Address,
    string? Phone,
    string? Email) : IRequest<ErrorOr<SupplierDto>>;
