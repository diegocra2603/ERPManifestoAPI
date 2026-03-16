using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.JournalEntries.Delete;

public record DeleteJournalEntryCommand(Guid Id) : IRequest<ErrorOr<bool>>;
