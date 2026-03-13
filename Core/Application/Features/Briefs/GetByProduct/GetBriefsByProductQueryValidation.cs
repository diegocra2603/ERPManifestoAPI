using FluentValidation;

namespace Application.Features.Briefs.GetByProduct;

public class GetBriefsByProductQueryValidation : AbstractValidator<GetBriefsByProductQuery>
{
    public GetBriefsByProductQueryValidation()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("El Id del producto es requerido.");
    }
}
