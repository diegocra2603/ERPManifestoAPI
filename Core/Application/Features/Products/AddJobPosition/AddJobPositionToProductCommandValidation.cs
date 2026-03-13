using FluentValidation;

namespace Application.Features.Products.AddJobPosition;

public class AddJobPositionToProductCommandValidation : AbstractValidator<AddJobPositionToProductCommand>
{
    public AddJobPositionToProductCommandValidation()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("El Id del producto es requerido.");
        RuleFor(x => x.JobPositionId).NotEmpty().WithMessage("El Id del puesto de trabajo es requerido.");
        RuleFor(x => x.Hours).GreaterThan(0).WithMessage("Las horas deben ser mayor a 0.");
    }
}
