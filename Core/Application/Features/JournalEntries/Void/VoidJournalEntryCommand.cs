using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.JournalEntries.Void;

public record VoidJournalEntryCommand(Guid Id) : IRequest<ErrorOr<bool>>;
