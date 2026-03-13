using FluentValidation;

namespace Application.Features.Briefs.Create;

public class CreateBriefCommandValidation : AbstractValidator<CreateBriefCommand>
{
    public CreateBriefCommandValidation()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("El Id del producto es requerido.");

        RuleFor(x => x.ClientName)
            .NotEmpty().WithMessage("El nombre del cliente es requerido.")
            .MaximumLength(300);

        RuleFor(x => x.ContactName)
            .NotEmpty().WithMessage("El nombre del contacto es requerido.")
            .MaximumLength(300);

        RuleFor(x => x.Date).NotEmpty().WithMessage("La fecha es requerida.");

        RuleFor(x => x.DurationMonths)
            .GreaterThan(0).When(x => x.DurationMonths.HasValue)
            .WithMessage("La duración debe ser mayor a 0 meses.");

        RuleFor(x => x.Budget)
            .GreaterThanOrEqualTo(0).When(x => x.Budget.HasValue)
            .WithMessage("El presupuesto debe ser mayor o igual a 0.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ItemName)
                .NotEmpty().WithMessage("El nombre del item es requerido.")
                .MaximumLength(500);

            item.RuleFor(i => i.SectionType)
                .IsInEnum().WithMessage("El tipo de sección no es válido.");
        });
    }
}
