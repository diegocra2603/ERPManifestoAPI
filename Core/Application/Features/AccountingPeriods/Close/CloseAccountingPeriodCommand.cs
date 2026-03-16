using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.AccountingPeriods.Close;

public record CloseAccountingPeriodCommand(Guid Id) : IRequest<ErrorOr<AccountingPeriodDto>>;
