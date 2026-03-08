using FluentValidation;

namespace Application.Features.Auth.LoginWithCode;

public class LoginWithCodeCommandValidation : AbstractValidator<LoginWithCodeCommand>
{
    public LoginWithCodeCommandValidation()
    {
        RuleFor(x => x.Code).NotEmpty().WithMessage("Code is required.");
        
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
    }
}