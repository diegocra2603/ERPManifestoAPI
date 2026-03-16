using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Invoices.GetPdf;

public record GetInvoicePdfQuery(Guid Id) : IRequest<ErrorOr<InvoicePdfResult>>;

public record InvoicePdfResult(byte[] FileBytes, string FileName);
