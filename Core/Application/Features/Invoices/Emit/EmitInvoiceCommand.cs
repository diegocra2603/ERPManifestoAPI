using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Invoices.Emit;

public record EmitInvoiceCommand(Guid Id) : IRequest<ErrorOr<InvoiceDto>>;
