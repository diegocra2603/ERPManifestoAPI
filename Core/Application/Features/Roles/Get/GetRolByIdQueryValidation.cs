using FluentValidation;

namespace Application.Features.Roles.Get;

public class GetRolByIdQueryValidation : AbstractValidator<GetRoleByIdQuery>
{
    public GetRolByIdQueryValidation()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required");
    }
}