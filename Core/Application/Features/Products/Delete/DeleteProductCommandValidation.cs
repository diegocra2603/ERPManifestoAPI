using FluentValidation;

namespace Application.Features.Products.Delete;

public class DeleteProductCommandValidation : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidation()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El Id es requerido.");
    }
}
