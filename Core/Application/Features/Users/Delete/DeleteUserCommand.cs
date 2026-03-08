using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Users.Delete;

public record DeleteUserCommand(Guid UserId) : IRequest<ErrorOr<Unit>>;
