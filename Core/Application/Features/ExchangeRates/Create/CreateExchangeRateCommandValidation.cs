using FluentValidation;

namespace Application.Features.ExchangeRates.Create;

public class CreateExchangeRateCommandValidation : AbstractValidator<CreateExchangeRateCommand>
{
    public CreateExchangeRateCommandValidation()
    {
        RuleFor(x => x.CurrencyId)
            .NotEmpty().WithMessage("La moneda es requerida.");

        RuleFor(x => x.BuyRate)
            .GreaterThan(0).WithMessage("La tasa de compra debe ser mayor a 0.");

        RuleFor(x => x.SellRate)
            .GreaterThan(0).WithMessage("La tasa de venta debe ser mayor a 0.");

        RuleFor(x => x.Source)
            .NotEmpty().WithMessage("La fuente es requerida.")
            .MaximumLength(100).WithMessage("Máximo 100 caracteres.");
    }
}
