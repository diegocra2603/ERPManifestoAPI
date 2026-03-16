using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Clients.GetAll;

public record GetAllClientsQuery : IRequest<ErrorOr<IReadOnlyList<ClientDto>>>;
