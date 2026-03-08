using FluentValidation;

namespace Application.Features.JobPositions.Get;

public class GetJobPositionByIdQueryValidation : AbstractValidator<GetJobPositionByIdQuery>
{
    public GetJobPositionByIdQueryValidation()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El Id es requerido.");
    }
}
