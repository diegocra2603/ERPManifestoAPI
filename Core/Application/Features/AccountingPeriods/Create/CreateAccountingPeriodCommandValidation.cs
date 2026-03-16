using FluentValidation;

namespace Application.Features.AccountingPeriods.Create;

public class CreateAccountingPeriodCommandValidation : AbstractValidator<CreateAccountingPeriodCommand>
{
    public CreateAccountingPeriodCommandValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("Máximo 100 caracteres.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("La fecha de inicio es requerida.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("La fecha de fin es requerida.")
            .GreaterThan(x => x.StartDate).WithMessage("La fecha de fin debe ser mayor a la fecha de inicio.");
    }
}
