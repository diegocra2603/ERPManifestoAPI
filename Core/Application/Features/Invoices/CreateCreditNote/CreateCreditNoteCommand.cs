using Application.Features.Invoices.CreateReceivable;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Invoices.CreateCreditNote;

public record CreateCreditNoteCommand(
    Guid OriginalInvoiceId,
    DateTime Date,
    string? Notes,
    List<CreateInvoiceItemDto> Items) : IRequest<ErrorOr<InvoiceDto>>;
