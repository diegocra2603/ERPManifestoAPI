using FluentValidation;

namespace Application.Features.JournalEntries.Create;

public class CreateJournalEntryCommandValidation : AbstractValidator<CreateJournalEntryCommand>
{
    public CreateJournalEntryCommandValidation()
    {
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("La fecha es requerida.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripci\u00f3n es requerida.")
            .MaximumLength(500).WithMessage("M\u00e1ximo 500 caracteres.");

        RuleFor(x => x.EntryType)
            .InclusiveBetween(1, 3).WithMessage("El tipo de partida debe ser entre 1 y 3.");

        RuleFor(x => x.PeriodId)
            .NotEmpty().WithMessage("El periodo es requerido.");

        RuleFor(x => x.CurrencyId)
            .NotEmpty().WithMessage("La moneda es requerida.");

        RuleFor(x => x.ExchangeRate)
            .GreaterThan(0).WithMessage("La tasa de cambio debe ser mayor a 0.");

        RuleFor(x => x.Lines)
            .NotEmpty().WithMessage("Debe incluir al menos una l\u00ednea.");

        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(l => l.AccountId)
                .NotEmpty().WithMessage("La cuenta contable es requerida.");

            line.RuleFor(l => l.Debit)
                .GreaterThanOrEqualTo(0).WithMessage("El d\u00e9bito debe ser mayor o igual a 0.");

            line.RuleFor(l => l.Credit)
                .GreaterThanOrEqualTo(0).WithMessage("El cr\u00e9dito debe ser mayor o igual a 0.");
        });
    }
}
