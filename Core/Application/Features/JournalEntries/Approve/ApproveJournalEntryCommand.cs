using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.JournalEntries.Approve;

public record ApproveJournalEntryCommand(Guid Id) : IRequest<ErrorOr<bool>>;
