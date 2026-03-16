using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.AccountingPeriods.Delete;

public record DeleteAccountingPeriodCommand(Guid Id) : IRequest<ErrorOr<bool>>;
