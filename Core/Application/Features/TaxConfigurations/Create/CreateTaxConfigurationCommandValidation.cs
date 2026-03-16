using FluentValidation;

namespace Application.Features.TaxConfigurations.Create;

public class CreateTaxConfigurationCommandValidation : AbstractValidator<CreateTaxConfigurationCommand>
{
    public CreateTaxConfigurationCommandValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("Máximo 100 caracteres.");

        RuleFor(x => x.Percentage)
            .GreaterThan(0).WithMessage("El porcentaje debe ser mayor a 0.")
            .LessThanOrEqualTo(100).WithMessage("El porcentaje no puede ser mayor a 100.");

        RuleFor(x => x.TaxType)
            .InclusiveBetween(1, 3).WithMessage("El tipo de impuesto debe ser entre 1 y 3.");

        RuleFor(x => x.DebitAccountId)
            .NotEmpty().WithMessage("La cuenta de débito es requerida.");

        RuleFor(x => x.CreditAccountId)
            .NotEmpty().WithMessage("La cuenta de crédito es requerida.");
    }
}
