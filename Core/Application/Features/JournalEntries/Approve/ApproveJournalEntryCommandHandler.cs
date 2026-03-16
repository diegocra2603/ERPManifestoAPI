using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Entities.Accounting.Enums;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.JournalEntries.Approve;

public class ApproveJournalEntryCommandHandler : IRequestHandler<ApproveJournalEntryCommand, ErrorOr<bool>>
{
    private readonly IAsyncRepository<JournalEntry> _journalEntryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveJournalEntryCommandHandler(
        IAsyncRepository<JournalEntry> journalEntryRepository,
        IUnitOfWork unitOfWork)
    {
        _journalEntryRepository = journalEntryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<bool>> Handle(ApproveJournalEntryCommand request, CancellationToken cancellationToken)
    {
        var entries = await _journalEntryRepository.GetAsync(
            predicate: e => e.Id == new JournalEntryId(request.Id) && e.AuditField.IsActive,
            includeString: "Lines");

        var entry = entries.FirstOrDefault();

        if (entry is null)
            return Error.NotFound("JournalEntry.NotFound", "Partida contable no encontrada.");

        if (entry.Status != JournalEntryStatus.Borrador)
            return Error.Validation("JournalEntry.NotDraft", "Solo se pueden aprobar partidas en estado Borrador.");

        if (!entry.IsBalanced)
            return Error.Validation("JournalEntry.NotBalanced", "La partida no cuadra. No se puede aprobar.");

        entry.Approve();
        _journalEntryRepository.Update(entry);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
