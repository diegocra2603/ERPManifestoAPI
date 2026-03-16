namespace Application.Features.Invoices;

public record InvoiceDto(
    Guid Id,
    int InvoiceType,
    string InvoiceTypeName,
    int Status,
    string StatusName,
    string InvoiceNumber,
    DateTime Date,
    DateTime? DueDate,
    Guid? ClientId,
    string? ClientName,
    Guid? SupplierId,
    string? SupplierName,
    string Nit,
    string Name,
    string? Address,
    Guid CurrencyId,
    string CurrencyCode,
    decimal ExchangeRate,
    decimal Subtotal,
    decimal TaxAmount,
    decimal Total,
    string? Notes,
    Guid? JournalEntryId,
    string? FiscalSerie,
    string? FiscalNumero,
    string? FiscalAutorizacion,
    IReadOnlyList<InvoiceItemDto> Items
);

public record InvoiceItemDto(
    Guid Id,
    string Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal Subtotal,
    decimal TaxAmount,
    decimal Total,
    int LineOrder
);
