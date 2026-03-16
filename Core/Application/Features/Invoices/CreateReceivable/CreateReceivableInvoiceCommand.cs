using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Invoices.CreateReceivable;

public record CreateReceivableInvoiceCommand(
    DateTime Date,
    DateTime? DueDate,
    Guid ClientId,
    Guid CurrencyId,
    decimal ExchangeRate,
    string? Notes,
    List<CreateInvoiceItemDto> Items) : IRequest<ErrorOr<InvoiceDto>>;

public record CreateInvoiceItemDto(
    string Description,
    decimal Quantity,
    decimal UnitPrice);
