using FluentValidation;

namespace Application.Features.ExchangeRates.Update;

public class UpdateExchangeRateCommandValidation : AbstractValidator<UpdateExchangeRateCommand>
{
    public UpdateExchangeRateCommandValidation()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El id es requerido.");

        RuleFor(x => x.BuyRate)
            .GreaterThan(0).WithMessage("La tasa de compra debe ser mayor a 0.");

        RuleFor(x => x.SellRate)
            .GreaterThan(0).WithMessage("La tasa de venta debe ser mayor a 0.");

        RuleFor(x => x.Source)
            .NotEmpty().WithMessage("La fuente es requerida.")
            .MaximumLength(100).WithMessage("Máximo 100 caracteres.");
    }
}
