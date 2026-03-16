using System.Linq.Expressions;
using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.JournalEntries.Get;

public class GetJournalEntryByIdQueryHandler : IRequestHandler<GetJournalEntryByIdQuery, ErrorOr<JournalEntryDto>>
{
    private readonly IAsyncRepository<JournalEntry> _journalEntryRepository;

    public GetJournalEntryByIdQueryHandler(IAsyncRepository<JournalEntry> journalEntryRepository)
    {
        _journalEntryRepository = journalEntryRepository;
    }

    public async Task<ErrorOr<JournalEntryDto>> Handle(GetJournalEntryByIdQuery request, CancellationToken cancellationToken)
    {
        var journalEntryId = new JournalEntryId(request.Id);

        var includes = new List<Expression<Func<JournalEntry, object>>>
        {
            e => e.Lines,
            e => e.Period,
            e => e.Currency
        };

        var entries = await _journalEntryRepository.GetAsync(
            predicate: e => e.Id == journalEntryId && e.AuditField.IsActive,
            includes: includes);

        var entry = entries.FirstOrDefault();

        if (entry is null)
            return Error.NotFound("JournalEntry.NotFound", "Partida contable no encontrada.");

        return new JournalEntryDto(
            entry.Id.Value,
            entry.EntryNumber,
            entry.Date,
            entry.Description,
            (int)entry.EntryType,
            entry.EntryType.ToString(),
            (int)entry.Status,
            entry.Status.ToString(),
            entry.PeriodId.Value,
            entry.Period.Name,
            entry.CurrencyId.Value,
            entry.Currency.Code,
            entry.ExchangeRate,
            entry.TotalDebit,
            entry.TotalCredit,
            entry.TotalDebitFunctional,
            entry.TotalCreditFunctional,
            entry.IsBalanced,
            entry.Lines.Select(l => new JournalEntryLineDto(
                l.Id.Value,
                l.AccountId.Value,
                l.Account?.AccountCode ?? "",
                l.Account?.Name ?? "",
                l.Description,
                l.Debit,
                l.Credit,
                l.DebitFunctional,
                l.CreditFunctional,
                l.LineOrder
            )).OrderBy(l => l.LineOrder).ToList().AsReadOnly()
        );
    }
}
