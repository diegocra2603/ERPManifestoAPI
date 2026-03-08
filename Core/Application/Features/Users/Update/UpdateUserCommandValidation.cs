using Domain.ValueObjects;
using FluentValidation;

namespace Application.Features.Users.Update;

public class UpdateUserCommandValidation : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidation()
    {
        RuleFor(x => x.Id).NotEmpty();

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
    }
}
