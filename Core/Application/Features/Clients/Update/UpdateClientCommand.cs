using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Clients.Update;

public record UpdateClientCommand(
    Guid Id,
    string Name,
    string? LegalName,
    string? Nit,
    string? Address,
    string? Phone,
    string? Email) : IRequest<ErrorOr<ClientDto>>;
