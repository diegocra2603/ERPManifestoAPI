using System.Linq.Expressions;
using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Entities.Accounting.Enums;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using Domain.ValueObjects;
using ErrorOr;

namespace Application.Features.JournalEntries.Create;

public class CreateJournalEntryCommandHandler : IRequestHandler<CreateJournalEntryCommand, ErrorOr<JournalEntryDto>>
{
    private readonly IAsyncRepository<JournalEntry> _journalEntryRepository;
    private readonly IAsyncRepository<AccountingPeriod> _periodRepository;
    private readonly IAsyncRepository<Currency> _currencyRepository;
    private readonly IAsyncRepository<AccountCatalog> _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateJournalEntryCommandHandler(
        IAsyncRepository<JournalEntry> journalEntryRepository,
        IAsyncRepository<AccountingPeriod> periodRepository,
        IAsyncRepository<Currency> currencyRepository,
        IAsyncRepository<AccountCatalog> accountRepository,
        IUnitOfWork unitOfWork)
    {
        _journalEntryRepository = journalEntryRepository;
        _periodRepository = periodRepository;
        _currencyRepository = currencyRepository;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    private static DateTime ToUtc(DateTime dt) =>
        dt.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(dt, DateTimeKind.Utc) : dt.ToUniversalTime();

    public async Task<ErrorOr<JournalEntryDto>> Handle(CreateJournalEntryCommand request, CancellationToken cancellationToken)
    {
        // Validate period exists and is open
        var period = await _periodRepository.FirstOrDefaultAsync(
            p => p.Id == new AccountingPeriodId(request.PeriodId) && p.AuditField.IsActive);

        if (period is null)
            return Error.NotFound("JournalEntry.PeriodNotFound", "Periodo contable no encontrado.");

        if (!period.IsOpen)
            return Error.Validation("JournalEntry.PeriodClosed", "El periodo contable no est\u00e1 abierto.");

        // Validate currency exists
        var currency = await _currencyRepository.FirstOrDefaultAsync(
            c => c.Id == new CurrencyId(request.CurrencyId) && c.AuditField.IsActive);

        if (currency is null)
            return Error.NotFound("JournalEntry.CurrencyNotFound", "Moneda no encontrada.");

        // Validate all accounts exist and accept movements
        foreach (var line in request.Lines)
        {
            var account = await _accountRepository.FirstOrDefaultAsync(
                a => a.Id == new AccountCatalogId(line.AccountId) && a.AuditField.IsActive);

            if (account is null)
                return Error.NotFound("JournalEntry.AccountNotFound",
                    $"Cuenta contable no encontrada: {line.AccountId}.");

            if (!account.AcceptsMovements)
                return Error.Validation("JournalEntry.AccountDoesNotAcceptMovements",
                    $"La cuenta '{account.AccountCode} - {account.Name}' no acepta movimientos.");
        }

        // Validate balance: sum of debits == sum of credits
        var totalDebit = request.Lines.Sum(l => l.Debit);
        var totalCredit = request.Lines.Sum(l => l.Credit);

        if (totalDebit != totalCredit)
            return Error.Validation("JournalEntry.NotBalanced", "La partida no cuadra.");

        // Get next entry number
        var existingEntries = await _journalEntryRepository.GetAsync(e => e.AuditField.IsActive);
        var entryNumber = existingEntries.Count + 1;

        var journalEntryId = new JournalEntryId(Guid.NewGuid());

        var journalEntry = new JournalEntry(
            journalEntryId,
            entryNumber,
            ToUtc(request.Date),
            request.Description,
            (JournalEntryType)request.EntryType,
            JournalEntryStatus.Borrador,
            new AccountingPeriodId(request.PeriodId),
            new CurrencyId(request.CurrencyId),
            request.ExchangeRate,
            AuditField.Create()
        );

        // Add lines
        for (var i = 0; i < request.Lines.Count; i++)
        {
            var lineDto = request.Lines[i];
            var line = new JournalEntryLine(
                new JournalEntryLineId(Guid.NewGuid()),
                journalEntryId,
                new AccountCatalogId(lineDto.AccountId),
                lineDto.Description,
                lineDto.Debit,
                lineDto.Credit,
                lineDto.Debit * request.ExchangeRate,
                lineDto.Credit * request.ExchangeRate,
                i + 1
            );
            journalEntry.AddLine(line);
        }

        _journalEntryRepository.Add(journalEntry);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload with includes to get navigation properties
        var includes = new List<Expression<Func<JournalEntry, object>>>
        {
            e => e.Lines,
            e => e.Period,
            e => e.Currency
        };

        var entries = await _journalEntryRepository.GetAsync(
            predicate: e => e.Id == journalEntryId && e.AuditField.IsActive,
            includes: includes);

        var saved = entries.First();

        return MapToDto(saved);
    }

    private static JournalEntryDto MapToDto(JournalEntry entry)
    {
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
                l.Account.AccountCode,
                l.Account.Name,
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
