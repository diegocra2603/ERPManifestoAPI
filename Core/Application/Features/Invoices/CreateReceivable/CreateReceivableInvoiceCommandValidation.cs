using FluentValidation;

namespace Application.Features.Invoices.CreateReceivable;

public class CreateReceivableInvoiceCommandValidation : AbstractValidator<CreateReceivableInvoiceCommand>
{
    public CreateReceivableInvoiceCommandValidation()
    {
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("La fecha es requerida.");

        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("El cliente es requerido.");

        RuleFor(x => x.CurrencyId)
            .NotEmpty().WithMessage("La moneda es requerida.");

        RuleFor(x => x.ExchangeRate)
            .GreaterThan(0).WithMessage("El tipo de cambio debe ser mayor a 0.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Debe incluir al menos un item.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.Description)
                .NotEmpty().WithMessage("La descripción del item es requerida.")
                .MaximumLength(500).WithMessage("La descripción del item no debe exceder 500 caracteres.");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.");

            item.RuleFor(i => i.UnitPrice)
                .GreaterThan(0).WithMessage("El precio unitario debe ser mayor a 0.");
        });
    }
}
