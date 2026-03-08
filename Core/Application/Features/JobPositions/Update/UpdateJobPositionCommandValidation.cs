using FluentValidation;

namespace Application.Features.JobPositions.Update;

public class UpdateJobPositionCommandValidation : AbstractValidator<UpdateJobPositionCommand>
{
    public UpdateJobPositionCommandValidation()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El Id es requerido.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es requerida.")
            .MaximumLength(500);

        RuleFor(x => x.HourlyCost)
            .GreaterThanOrEqualTo(0).WithMessage("El costo por hora debe ser mayor o igual a 0.");
    }
}
