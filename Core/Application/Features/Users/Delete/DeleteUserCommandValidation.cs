using FluentValidation;

namespace Application.Features.Users.Delete;

public class DeleteUserCommandValidation : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidation()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
