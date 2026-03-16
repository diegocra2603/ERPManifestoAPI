using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using Domain.ValueObjects;
using ErrorOr;

namespace Application.Features.AccountingPeriods.Update;

public class UpdateAccountingPeriodCommandHandler : IRequestHandler<UpdateAccountingPeriodCommand, ErrorOr<AccountingPeriodDto>>
{
    private readonly IAsyncRepository<AccountingPeriod> _periodRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAccountingPeriodCommandHandler(
        IAsyncRepository<AccountingPeriod> periodRepository,
        IUnitOfWork unitOfWork)
    {
        _periodRepository = periodRepository;
        _unitOfWork = unitOfWork;
    }

    private static DateTime ToUtc(DateTime dt) =>
        dt.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(dt, DateTimeKind.Utc) : dt.ToUniversalTime();

    public async Task<ErrorOr<AccountingPeriodDto>> Handle(UpdateAccountingPeriodCommand request, CancellationToken cancellationToken)
    {
        var period = await _periodRepository.FirstOrDefaultAsync(p => p.Id == new AccountingPeriodId(request.Id) && p.AuditField.IsActive);
        if (period is null)
            return Error.NotFound("AccountingPeriod.NotFound", "Período contable no encontrado.");

        if (!period.IsOpen)
            return Error.Validation("AccountingPeriod.NotOpen", "Solo se pueden modificar períodos abiertos.");

        var overlapping = await _periodRepository.ExistsAsync(p =>
            p.StartDate <= request.EndDate &&
            p.EndDate >= request.StartDate &&
            p.Id != new AccountingPeriodId(request.Id) &&
            p.AuditField.IsActive);

        if (overlapping)
            return Error.Validation("AccountingPeriod.Overlapping", "Ya existe otro período contable que se traslapa con las fechas indicadas.");

        period.Update(request.Name, ToUtc(request.StartDate), ToUtc(request.EndDate));
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
