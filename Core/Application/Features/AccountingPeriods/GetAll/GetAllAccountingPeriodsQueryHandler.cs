using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.AccountingPeriods.GetAll;

public class GetAllAccountingPeriodsQueryHandler : IRequestHandler<GetAllAccountingPeriodsQuery, ErrorOr<IReadOnlyList<AccountingPeriodDto>>>
{
    private readonly IAsyncRepository<AccountingPeriod> _periodRepository;

    public GetAllAccountingPeriodsQueryHandler(IAsyncRepository<AccountingPeriod> periodRepository)
    {
        _periodRepository = periodRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<AccountingPeriodDto>>> Handle(GetAllAccountingPeriodsQuery request, CancellationToken cancellationToken)
    {
        var periods = await _periodRepository.GetAsync(
            predicate: p => p.AuditField.IsActive,
            orderBy: q => q.OrderByDescending(p => p.StartDate),
            includeString: null);

        return periods
            .Select(p => new AccountingPeriodDto(
                p.Id.Value,
                p.Name,
                p.StartDate,
                p.EndDate,
                (int)p.Status,
                p.Status.ToString()))
            .ToList()
            .AsReadOnly();
    }
}
