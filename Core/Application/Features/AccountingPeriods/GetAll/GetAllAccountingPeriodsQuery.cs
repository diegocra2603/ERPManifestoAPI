using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.AccountingPeriods.GetAll;

public record GetAllAccountingPeriodsQuery : IRequest<ErrorOr<IReadOnlyList<AccountingPeriodDto>>>;
