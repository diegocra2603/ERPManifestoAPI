using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.JournalEntries.Get;

public record GetJournalEntryByIdQuery(Guid Id) : IRequest<ErrorOr<JournalEntryDto>>;
