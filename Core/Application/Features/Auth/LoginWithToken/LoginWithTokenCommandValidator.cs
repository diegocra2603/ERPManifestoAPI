using FluentValidation;

namespace Application.Features.Auth.LoginWithToken;

public class LoginWithTokenCommandValidator : AbstractValidator<LoginWithTokenCommand>
{
    public LoginWithTokenCommandValidator()
    {
        // Sin reglas: el usuario viene del token [Authorize]
    }
}
