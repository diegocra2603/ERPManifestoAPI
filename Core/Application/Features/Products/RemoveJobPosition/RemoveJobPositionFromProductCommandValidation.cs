using FluentValidation;

namespace Application.Features.Products.RemoveJobPosition;

public class RemoveJobPositionFromProductCommandValidation : AbstractValidator<RemoveJobPositionFromProductCommand>
{
    public RemoveJobPositionFromProductCommandValidation()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("El Id del producto es requerido.");
        RuleFor(x => x.ProductJobPositionId).NotEmpty().WithMessage("El Id del puesto asignado es requerido.");
    }
}
