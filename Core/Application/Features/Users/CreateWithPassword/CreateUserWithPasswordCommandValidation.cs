using Domain.ValueObjects;
using FluentValidation;

namespace Application.Features.Users.CreateWithPassword;

public class CreateUserWithPasswordCommandValidation : AbstractValidator<CreateUserWithPasswordCommand>
{
    public CreateUserWithPasswordCommandValidation()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(Email.MaxLength);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(Name.MaxLength);

        RuleFor(x => x.Code).NotEmpty();

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MaximumLength(PhoneNumber.MaxLength);

        RuleFor(x => x.RoleId).NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .WithMessage("La contraseña debe tener al menos 6 caracteres.");
    }
}
