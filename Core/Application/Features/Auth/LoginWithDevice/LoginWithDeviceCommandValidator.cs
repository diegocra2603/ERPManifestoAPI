using FluentValidation;

namespace Application.Features.Auth.LoginWithDevice;

public class LoginWithDeviceCommandValidator : AbstractValidator<LoginWithDeviceCommand>
{
    public LoginWithDeviceCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es requerido.")
            .EmailAddress().WithMessage("El formato del correo electrónico no es válido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida.")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");

        RuleFor(x => x.DeviceId)
            .NotEmpty().WithMessage("El identificador del dispositivo es requerido.");
    }
}
