using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.AccountingPeriods.Update;

public record UpdateAccountingPeriodCommand(
    Guid Id,
    string Name,
    DateTime StartDate,
    DateTime EndDate) : IRequest<ErrorOr<AccountingPeriodDto>>;
