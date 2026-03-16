using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Invoices.Get;

public record GetInvoiceByIdQuery(Guid Id) : IRequest<ErrorOr<InvoiceDto>>;
