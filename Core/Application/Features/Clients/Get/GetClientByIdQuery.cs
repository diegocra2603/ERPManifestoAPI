using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Clients.Get;

public record GetClientByIdQuery(Guid Id) : IRequest<ErrorOr<ClientDto>>;
