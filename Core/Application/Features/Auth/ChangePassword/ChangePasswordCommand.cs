using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Auth.ChangePassword;

public record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword
) : IRequest<ErrorOr<bool>>;
