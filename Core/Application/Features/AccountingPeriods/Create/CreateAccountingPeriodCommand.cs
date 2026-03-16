using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.AccountingPeriods.Create;

public record CreateAccountingPeriodCommand(
    string Name,
    DateTime StartDate,
    DateTime EndDate) : IRequest<ErrorOr<AccountingPeriodDto>>;
