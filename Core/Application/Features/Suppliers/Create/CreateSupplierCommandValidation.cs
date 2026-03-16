using FluentValidation;

namespace Application.Features.Suppliers.Create;

public class CreateSupplierCommandValidation : AbstractValidator<CreateSupplierCommand>
{
    public CreateSupplierCommandValidation()
    {
        RuleFor(x => x.Nit)
            .NotEmpty().WithMessage("El NIT es requerido.")
            .MaximumLength(50).WithMessage("Máximo 50 caracteres.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(500).WithMessage("Máximo 500 caracteres.");
    }
}
