using FluentValidation;

namespace Application.Features.AccountCatalogs.Update;

public class UpdateAccountCatalogCommandValidation : AbstractValidator<UpdateAccountCatalogCommand>
{
    public UpdateAccountCatalogCommandValidation()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El id es requerido.");

        RuleFor(x => x.AccountCode)
            .NotEmpty().WithMessage("El código de cuenta es requerido.")
            .MaximumLength(50).WithMessage("Máximo 50 caracteres.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(200).WithMessage("Máximo 200 caracteres.");

        RuleFor(x => x.AccountType)
            .InclusiveBetween(1, 6).WithMessage("El tipo de cuenta debe estar entre 1 y 6.");

        RuleFor(x => x.Nature)
            .InclusiveBetween(1, 2).WithMessage("La naturaleza debe estar entre 1 y 2.");

        RuleFor(x => x.Level)
            .GreaterThanOrEqualTo(1).WithMessage("El nivel debe ser mayor o igual a 1.");
    }
}
