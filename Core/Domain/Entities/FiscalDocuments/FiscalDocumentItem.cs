using Domain.Primitives;

namespace Domain.Entities.FiscalDocuments;

public sealed class FiscalDocumentItem : Entity
{
    private FiscalDocumentItem() { }

    public FiscalDocumentItem(
        FiscalDocumentItemId id,
        FiscalDocumentId fiscalDocumentId,
        string productCode,
        string description,
        int measureUnit,
        decimal quantity,
        decimal price,
        decimal discountPercentage,
        decimal grossAmount,
        decimal discountAmount,
        decimal exemptAmount,
        decimal otherTaxes,
        decimal netAmount,
        decimal isrAmount,
        decimal ivaAmount,
        decimal totalAmount,
        SaleType saleType)
    {
        Id = id;
        FiscalDocumentId = fiscalDocumentId;
        ProductCode = productCode;
        Description = description;
        MeasureUnit = measureUnit;
        Quantity = quantity;
        Price = price;
        DiscountPercentage = discountPercentage;
        GrossAmount = grossAmount;
        DiscountAmount = discountAmount;
        ExemptAmount = exemptAmount;
        OtherTaxes = otherTaxes;
        NetAmount = netAmount;
        IsrAmount = isrAmount;
        IvaAmount = ivaAmount;
        TotalAmount = totalAmount;
        SaleType = saleType;
    }

    public FiscalDocumentItemId Id { get; private set; } = default!;
    public FiscalDocumentId FiscalDocumentId { get; private set; } = default!;
    public string ProductCode { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public int MeasureUnit { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal Price { get; private set; }
    public decimal DiscountPercentage { get; private set; }
    public decimal GrossAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal ExemptAmount { get; private set; }
    public decimal OtherTaxes { get; private set; }
    public decimal NetAmount { get; private set; }
    public decimal IsrAmount { get; private set; }
    public decimal IvaAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public SaleType SaleType { get; private set; }

    public FiscalDocument FiscalDocument { get; private set; } = default!;
}
