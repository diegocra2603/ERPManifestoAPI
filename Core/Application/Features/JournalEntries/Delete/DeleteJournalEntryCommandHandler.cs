using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Entities.Accounting.Enums;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.JournalEntries.Delete;

public class DeleteJournalEntryCommandHandler : IRequestHandler<DeleteJournalEntryCommand, ErrorOr<bool>>
{
    private readonly IAsyncRepository<JournalEntry> _journalEntryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteJournalEntryCommandHandler(
        IAsyncRepository<JournalEntry> journalEntryRepository,
        IUnitOfWork unitOfWork)
    {
        _journalEntryRepository = journalEntryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<bool>> Handle(DeleteJournalEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = await _journalEntryRepository.FirstOrDefaultAsync(
            e => e.Id == new JournalEntryId(request.Id) && e.AuditField.IsActive);

        if (entry is null)
            return Error.NotFound("JournalEntry.NotFound", "Partida contable no encontrada.");

        if (entry.Status != JournalEntryStatus.Borrador)
            return Error.Validation("JournalEntry.CannotDelete", "Solo se pueden eliminar partidas en estado Borrador.");

        entry.MarkAsDeleted();
        _journalEntryRepository.Update(entry);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
