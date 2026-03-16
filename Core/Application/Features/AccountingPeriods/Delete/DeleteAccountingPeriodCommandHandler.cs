using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using Domain.ValueObjects;
using ErrorOr;

namespace Application.Features.AccountingPeriods.Delete;

public class DeleteAccountingPeriodCommandHandler : IRequestHandler<DeleteAccountingPeriodCommand, ErrorOr<bool>>
{
    private readonly IAsyncRepository<AccountingPeriod> _periodRepository;
    private readonly IAsyncRepository<JournalEntry> _journalEntryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAccountingPeriodCommandHandler(
        IAsyncRepository<AccountingPeriod> periodRepository,
        IAsyncRepository<JournalEntry> journalEntryRepository,
        IUnitOfWork unitOfWork)
    {
        _periodRepository = periodRepository;
        _journalEntryRepository = journalEntryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<bool>> Handle(DeleteAccountingPeriodCommand request, CancellationToken cancellationToken)
    {
        var period = await _periodRepository.FirstOrDefaultAsync(p => p.Id == new AccountingPeriodId(request.Id) && p.AuditField.IsActive);
        if (period is null)
            return Error.NotFound("AccountingPeriod.NotFound", "Período contable no encontrado.");

        var hasEntries = await _journalEntryRepository.ExistsAsync(j => j.PeriodId == new AccountingPeriodId(request.Id) && j.AuditField.IsActive);
        if (hasEntries)
            return Error.Validation("AccountingPeriod.HasJournalEntries", "No se puede eliminar el período porque tiene asientos contables asociados.");

        period.MarkAsDeleted();
        _periodRepository.Update(period);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
