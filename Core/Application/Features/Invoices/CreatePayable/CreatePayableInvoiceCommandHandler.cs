using System.Linq.Expressions;
using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Entities.Accounting.Enums;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using Domain.ValueObjects;
using ErrorOr;

namespace Application.Features.Invoices.CreatePayable;

public class CreatePayableInvoiceCommandHandler : IRequestHandler<CreatePayableInvoiceCommand, ErrorOr<InvoiceDto>>
{
    private readonly IAsyncRepository<Invoice> _invoiceRepository;
    private readonly IAsyncRepository<Supplier> _supplierRepository;
    private readonly IAsyncRepository<Currency> _currencyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePayableInvoiceCommandHandler(
        IAsyncRepository<Invoice> invoiceRepository,
        IAsyncRepository<Supplier> supplierRepository,
        IAsyncRepository<Currency> currencyRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _supplierRepository = supplierRepository;
        _currencyRepository = currencyRepository;
        _unitOfWork = unitOfWork;
    }

    private static DateTime ToUtc(DateTime dt) =>
        dt.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(dt, DateTimeKind.Utc) : dt.ToUniversalTime();

    public async Task<ErrorOr<InvoiceDto>> Handle(CreatePayableInvoiceCommand request, CancellationToken cancellationToken)
    {
        // Validate supplier exists
        var supplier = await _supplierRepository.FirstOrDefaultAsync(
            s => s.Id == new SupplierId(request.SupplierId) && s.AuditField.IsActive);

        if (supplier is null)
            return Error.NotFound("Invoice.SupplierNotFound", "Proveedor no encontrado.");

        // Validate currency exists
        var currency = await _currencyRepository.FirstOrDefaultAsync(
            c => c.Id == new CurrencyId(request.CurrencyId) && c.AuditField.IsActive);

        if (currency is null)
            return Error.NotFound("Invoice.CurrencyNotFound", "Moneda no encontrada.");

        // Generate invoice number
        var existingInvoices = await _invoiceRepository.GetAsync(
            i => i.InvoiceType == InvoiceType.Payable && i.AuditField.IsActive);
        var invoiceNumber = "COMP-" + (existingInvoices.Count + 1).ToString("D6");

        var invoiceId = new InvoiceId(Guid.NewGuid());

        // Calculate items
        decimal invoiceSubtotal = 0;
        decimal invoiceTaxAmount = 0;
        decimal invoiceTotal = 0;

        var itemEntities = new List<InvoiceItem>();

        for (var i = 0; i < request.Items.Count; i++)
        {
            var itemDto = request.Items[i];
            var subtotal = itemDto.Quantity * itemDto.UnitPrice;
            var taxAmount = Math.Round(subtotal * 0.12m, 2);
            var total = subtotal + taxAmount;

            invoiceSubtotal += subtotal;
            invoiceTaxAmount += taxAmount;
            invoiceTotal += total;

            var item = new InvoiceItem(
                new InvoiceItemId(Guid.NewGuid()),
                invoiceId,
                itemDto.Description,
                itemDto.Quantity,
                itemDto.UnitPrice,
                subtotal,
                taxAmount,
                total,
                i + 1
            );

            itemEntities.Add(item);
        }

        var invoice = new Invoice(
            invoiceId,
            InvoiceType.Payable,
            InvoiceStatus.Borrador,
            invoiceNumber,
            ToUtc(request.Date),
            request.DueDate.HasValue ? ToUtc(request.DueDate.Value) : null,
            null,
            new SupplierId(request.SupplierId),
            supplier.Nit,
            supplier.Name,
            supplier.Address,
            new CurrencyId(request.CurrencyId),
            request.ExchangeRate,
            invoiceSubtotal,
            invoiceTaxAmount,
            invoiceTotal,
            request.Notes,
            null,
            request.FiscalSerie,
            request.FiscalNumero,
            request.FiscalAutorizacion,
            AuditField.Create()
        );

        foreach (var item in itemEntities)
        {
            invoice.AddItem(item);
        }

        _invoiceRepository.Add(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload with includes
        var includes = new List<Expression<Func<Invoice, object>>>
        {
            e => e.Supplier!,
            e => e.Currency,
            e => e.Items
        };

        var invoices = await _invoiceRepository.GetAsync(
            predicate: e => e.Id == invoiceId && e.AuditField.IsActive,
            includes: includes);

        var saved = invoices.First();

        return InvoiceMapper.MapToDto(saved);
    }
}
