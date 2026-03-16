using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Clients.Create;

public record CreateClientCommand(
    string Name,
    string? LegalName,
    string? Nit,
    string? Address,
    string? Phone,
    string? Email) : IRequest<ErrorOr<ClientDto>>;
