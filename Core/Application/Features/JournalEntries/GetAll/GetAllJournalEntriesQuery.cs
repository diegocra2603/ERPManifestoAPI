using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.JournalEntries.GetAll;

public record GetAllJournalEntriesQuery : IRequest<ErrorOr<IReadOnlyList<JournalEntryDto>>>;
