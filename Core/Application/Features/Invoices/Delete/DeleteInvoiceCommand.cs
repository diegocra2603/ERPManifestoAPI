using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Invoices.Delete;

public record DeleteInvoiceCommand(Guid Id) : IRequest<ErrorOr<bool>>;
