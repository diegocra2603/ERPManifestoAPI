using FluentValidation;

namespace Application.Features.Currencies.Create;

public class CreateCurrencyCommandValidation : AbstractValidator<CreateCurrencyCommand>
{
    public CreateCurrencyCommandValidation()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("El código es requerido.")
            .MaximumLength(10).WithMessage("Máximo 10 caracteres.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("Máximo 100 caracteres.");

        RuleFor(x => x.Symbol)
            .NotEmpty().WithMessage("El símbolo es requerido.")
            .MaximumLength(10).WithMessage("Máximo 10 caracteres.");

        RuleFor(x => x.DecimalPlaces)
            .InclusiveBetween(0, 6).WithMessage("Los decimales deben estar entre 0 y 6.");
    }
}
