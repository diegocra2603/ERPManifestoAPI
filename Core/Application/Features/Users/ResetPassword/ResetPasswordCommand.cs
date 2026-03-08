using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Users.ResetPassword;

public record ResetPasswordCommand(Guid? UserId = null) : IRequest<ErrorOr<Unit>>;
