using FluentValidation;

namespace Application.Features.JobPositions.Delete;

public class DeleteJobPositionCommandValidation : AbstractValidator<DeleteJobPositionCommand>
{
    public DeleteJobPositionCommandValidation()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El Id es requerido.");
    }
}
