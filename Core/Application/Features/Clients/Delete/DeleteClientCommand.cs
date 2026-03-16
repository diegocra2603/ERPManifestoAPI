using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Clients.Delete;

public record DeleteClientCommand(Guid Id) : IRequest<ErrorOr<bool>>;
