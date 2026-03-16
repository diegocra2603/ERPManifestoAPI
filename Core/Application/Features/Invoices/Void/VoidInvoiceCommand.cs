using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Invoices.Void;

public record VoidInvoiceCommand(Guid Id) : IRequest<ErrorOr<bool>>;
