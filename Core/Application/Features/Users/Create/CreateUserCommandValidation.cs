using Domain.ValueObjects;
using FluentValidation;

namespace Application.Features.Users.Create;

public class CreateUserCommandValidation : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidation()
    {
        RuleFor(x => x.Email)
        .NotEmpty()
        .EmailAddress()
        .MaximumLength(Email.MaxLength);

        RuleFor(x => x.Name)
        .NotEmpty()
        .MaximumLength(Name.MaxLength);

        RuleFor(x => x.Code)
        .NotEmpty();

        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(PhoneNumber.MaxLength);

        RuleFor(x => x.RoleId).NotEmpty();
    }
}