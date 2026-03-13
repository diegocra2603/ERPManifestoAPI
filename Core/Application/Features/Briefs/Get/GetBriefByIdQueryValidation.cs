using FluentValidation;

namespace Application.Features.Briefs.Get;

public class GetBriefByIdQueryValidation : AbstractValidator<GetBriefByIdQuery>
{
    public GetBriefByIdQueryValidation()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El Id es requerido.");
    }
}
