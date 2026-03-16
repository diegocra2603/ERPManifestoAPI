using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.JournalEntries.Create;

public record CreateJournalEntryCommand(
    DateTime Date,
    string Description,
    int EntryType,
    Guid PeriodId,
    Guid CurrencyId,
    decimal ExchangeRate,
    List<CreateJournalEntryLineDto> Lines) : IRequest<ErrorOr<JournalEntryDto>>;

public record CreateJournalEntryLineDto(
    Guid AccountId,
    string Description,
    decimal Debit,
    decimal Credit);
