using FluentValidation;

namespace Application.Features.Users.ResetPassword;

public class ResetPasswordCommandValidation : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidation()
    {
        RuleFor(x => x.UserId)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage("UserId no puede ser vacío cuando se proporciona.");
    }
}
