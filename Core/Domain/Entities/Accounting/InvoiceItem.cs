using Domain.Primitives;

namespace Domain.Entities.Accounting;

public sealed class InvoiceItem : Entity
{
    private InvoiceItem() { }

    public InvoiceItem(
        InvoiceItemId id,
        InvoiceId invoiceId,
        string description,
        decimal quantity,
        decimal unitPrice,
        decimal subtotal,
        decimal taxAmount,
        decimal total,
        int lineOrder)
    {
        Id = id;
        InvoiceId = invoiceId;
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Subtotal = subtotal;
        TaxAmount = taxAmount;
        Total = total;
        LineOrder = lineOrder;
    }

    public InvoiceItemId Id { get; private set; } = default!;
    public InvoiceId InvoiceId { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal Total { get; private set; }
    public int LineOrder { get; private set; }

    // Navigation
    public Invoice Invoice { get; private set; } = default!;
}
