namespace Application.Features.JournalEntries;

public record JournalEntryDto(
    Guid Id,
    int EntryNumber,
    DateTime Date,
    string Description,
    int EntryType,
    string EntryTypeName,
    int Status,
    string StatusName,
    Guid PeriodId,
    string PeriodName,
    Guid CurrencyId,
    string CurrencyCode,
    decimal ExchangeRate,
    decimal TotalDebit,
    decimal TotalCredit,
    decimal TotalDebitFunctional,
    decimal TotalCreditFunctional,
    bool IsBalanced,
    IReadOnlyList<JournalEntryLineDto> Lines
);

public record JournalEntryLineDto(
    Guid Id,
    Guid AccountId,
    string AccountCode,
    string AccountName,
    string Description,
    decimal Debit,
    decimal Credit,
    decimal DebitFunctional,
    decimal CreditFunctional,
    int LineOrder
);
