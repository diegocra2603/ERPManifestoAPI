using FluentValidation;

namespace Application.Features.Products.Get;

public class GetProductByIdQueryValidation : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdQueryValidation()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El Id es requerido.");
    }
}
