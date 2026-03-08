using FluentValidation;

namespace Application.Features.Users.Get;

public class GetUserByParamQueryValidation : AbstractValidator<GetUserByParamQuery>
{
    public GetUserByParamQueryValidation()
    {
        RuleFor(x => x.Param).NotEmpty().WithMessage("Param is required");
    }
}
