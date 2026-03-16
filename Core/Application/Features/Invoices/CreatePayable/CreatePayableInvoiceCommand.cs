using Application.Features.Invoices.CreateReceivable;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Invoices.CreatePayable;

public record CreatePayableInvoiceCommand(
    DateTime Date,
    DateTime? DueDate,
    Guid SupplierId,
    Guid CurrencyId,
    decimal ExchangeRate,
    string? FiscalSerie,
    string? FiscalNumero,
    string? FiscalAutorizacion,
    string? Notes,
    List<CreateInvoiceItemDto> Items) : IRequest<ErrorOr<InvoiceDto>>;
