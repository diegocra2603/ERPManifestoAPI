using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using Domain.ValueObjects;
using ErrorOr;

namespace Application.Features.AccountingPeriods.Close;

public class CloseAccountingPeriodCommandHandler : IRequestHandler<CloseAccountingPeriodCommand, ErrorOr<AccountingPeriodDto>>
{
    private readonly IAsyncRepository<AccountingPeriod> _periodRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CloseAccountingPeriodCommandHandler(
        IAsyncRepository<AccountingPeriod> periodRepository,
        IUnitOfWork unitOfWork)
    {
        _periodRepository = periodRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<AccountingPeriodDto>> Handle(CloseAccountingPeriodCommand request, CancellationToken cancellationToken)
    {
        var period = await _periodRepository.FirstOrDefaultAsync(p => p.Id == new AccountingPeriodId(request.Id) && p.AuditField.IsActive);
        if (period is null)
            return Error.NotFound("AccountingPeriod.NotFound", "Período contable no encontrado.");

        if (!period.IsOpen)
            return Error.Validation("AccountingPeriod.NotOpen", "El período contable ya se encuentra cerrado.");

        period.Close();
        _periodRepository.Update(period);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AccountingPeriodDto(
            period.Id.Value,
            period.Name,
            period.StartDate,
            period.EndDate,
            (int)period.Status,
            period.Status.ToString()
        );
    }
}
