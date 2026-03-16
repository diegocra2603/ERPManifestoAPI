using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Entities.Accounting.Enums;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using Domain.ValueObjects;
using ErrorOr;

namespace Application.Features.AccountingPeriods.Create;

public class CreateAccountingPeriodCommandHandler : IRequestHandler<CreateAccountingPeriodCommand, ErrorOr<AccountingPeriodDto>>
{
    private readonly IAsyncRepository<AccountingPeriod> _periodRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAccountingPeriodCommandHandler(
        IAsyncRepository<AccountingPeriod> periodRepository,
        IUnitOfWork unitOfWork)
    {
        _periodRepository = periodRepository;
        _unitOfWork = unitOfWork;
    }

    private static DateTime ToUtc(DateTime dt) =>
        dt.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(dt, DateTimeKind.Utc) : dt.ToUniversalTime();

    public async Task<ErrorOr<AccountingPeriodDto>> Handle(CreateAccountingPeriodCommand request, CancellationToken cancellationToken)
    {
        var overlapping = await _periodRepository.ExistsAsync(p =>
            p.StartDate <= request.EndDate &&
            p.EndDate >= request.StartDate &&
            p.AuditField.IsActive);

        if (overlapping)
            return Error.Validation("AccountingPeriod.Overlapping", "Ya existe un período contable que se traslapa con las fechas indicadas.");

        var status = PeriodStatus.Abierto;

        var period = new AccountingPeriod(
            new AccountingPeriodId(Guid.NewGuid()),
            request.Name,
            ToUtc(request.StartDate),
            ToUtc(request.EndDate),
            status,
            AuditField.Create()
        );

        _periodRepository.Add(period);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AccountingPeriodDto(
            period.Id.Value,
            period.Name,
            period.StartDate,
            period.EndDate,
            (int)period.Status,
            status.ToString()
        );
    }
}
