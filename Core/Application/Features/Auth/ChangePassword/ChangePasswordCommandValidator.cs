using FluentValidation;

namespace Application.Features.Auth.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("La contraseña actual es requerida.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("La nueva contraseña es requerida.")
            .MinimumLength(8).WithMessage("La nueva contraseña debe tener al menos 8 caracteres.")
            .Matches("[A-Z]").WithMessage("La nueva contraseña debe contener al menos una letra mayúscula.")
            .Matches("[a-z]").WithMessage("La nueva contraseña debe contener al menos una letra minúscula.")
            .Matches("[0-9]").WithMessage("La nueva contraseña debe contener al menos un número.")
            .Matches("[^a-zA-Z0-9]").WithMessage("La nueva contraseña debe contener al menos un carácter especial.");

        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty().WithMessage("La confirmación de la nueva contraseña es requerida.")
            .Equal(x => x.NewPassword).WithMessage("Las contraseñas no coinciden.");

        RuleFor(x => x.NewPassword)
            .NotEqual(x => x.CurrentPassword).WithMessage("La nueva contraseña debe ser diferente a la contraseña actual.");
    }
}
