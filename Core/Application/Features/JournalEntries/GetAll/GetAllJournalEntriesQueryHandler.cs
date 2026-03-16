using System.Linq.Expressions;
using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.JournalEntries.GetAll;

public class GetAllJournalEntriesQueryHandler : IRequestHandler<GetAllJournalEntriesQuery, ErrorOr<IReadOnlyList<JournalEntryDto>>>
{
    private readonly IAsyncRepository<JournalEntry> _journalEntryRepository;

    public GetAllJournalEntriesQueryHandler(IAsyncRepository<JournalEntry> journalEntryRepository)
    {
        _journalEntryRepository = journalEntryRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<JournalEntryDto>>> Handle(GetAllJournalEntriesQuery request, CancellationToken cancellationToken)
    {
        var includes = new List<Expression<Func<JournalEntry, object>>>
        {
            e => e.Lines,
            e => e.Period,
            e => e.Currency
        };

        var entries = await _journalEntryRepository.GetAsync(
            predicate: e => e.AuditField.IsActive,
            includes: includes);

        return entries
            .Select(e => new JournalEntryDto(
                e.Id.Value,
                e.EntryNumber,
                e.Date,
                e.Description,
                (int)e.EntryType,
                e.EntryType.ToString(),
                (int)e.Status,
                e.Status.ToString(),
                e.PeriodId.Value,
                e.Period.Name,
                e.CurrencyId.Value,
                e.Currency.Code,
                e.ExchangeRate,
                e.TotalDebit,
                e.TotalCredit,
                e.TotalDebitFunctional,
                e.TotalCreditFunctional,
                e.IsBalanced,
                e.Lines.Select(l => new JournalEntryLineDto(
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
            ))
            .ToList()
            .AsReadOnly();
    }
}
