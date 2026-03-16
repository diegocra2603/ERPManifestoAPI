using System.Linq.Expressions;
using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Entities.Accounting.Enums;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using Domain.ValueObjects;
using ErrorOr;

namespace Application.Features.Invoices.CreateReceivable;

public class CreateReceivableInvoiceCommandHandler : IRequestHandler<CreateReceivableInvoiceCommand, ErrorOr<InvoiceDto>>
{
    private readonly IAsyncRepository<Invoice> _invoiceRepository;
    private readonly IAsyncRepository<Client> _clientRepository;
    private readonly IAsyncRepository<Currency> _currencyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReceivableInvoiceCommandHandler(
        IAsyncRepository<Invoice> invoiceRepository,
        IAsyncRepository<Client> clientRepository,
        IAsyncRepository<Currency> currencyRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _clientRepository = clientRepository;
        _currencyRepository = currencyRepository;
        _unitOfWork = unitOfWork;
    }

    private static DateTime ToUtc(DateTime dt) =>
        dt.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(dt, DateTimeKind.Utc) : dt.ToUniversalTime();

    public async Task<ErrorOr<InvoiceDto>> Handle(CreateReceivableInvoiceCommand request, CancellationToken cancellationToken)
    {
        var client = await _clientRepository.FirstOrDefaultAsync(
            c => c.Id == new ClientId(request.ClientId) && c.AuditField.IsActive);

        if (client is null)
            return Error.NotFound("Invoice.ClientNotFound", "Cliente no encontrado.");

        var currency = await _currencyRepository.FirstOrDefaultAsync(
            c => c.Id == new CurrencyId(request.CurrencyId) && c.AuditField.IsActive);

        if (currency is null)
            return Error.NotFound("Invoice.CurrencyNotFound", "Moneda no encontrada.");

        var existingInvoices = await _invoiceRepository.GetAsync(
            i => i.InvoiceType == InvoiceType.Receivable && i.AuditField.IsActive);
        var invoiceNumber = "FAC-" + (existingInvoices.Count + 1).ToString("D6");

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

            itemEntities.Add(new InvoiceItem(
                new InvoiceItemId(Guid.NewGuid()),
                invoiceId,
                itemDto.Description,
                itemDto.Quantity,
                itemDto.UnitPrice,
                subtotal, taxAmount, total,
                i + 1
            ));
        }

        var invoice = new Invoice(
            invoiceId,
            InvoiceType.Receivable,
            InvoiceStatus.Borrador,
            invoiceNumber,
            ToUtc(request.Date),
            request.DueDate.HasValue ? ToUtc(request.DueDate.Value) : null,
            new ClientId(request.ClientId),
            null,
            client.Nit ?? "CF",
            client.Name,
            client.Address,
            new CurrencyId(request.CurrencyId),
            request.ExchangeRate,
            invoiceSubtotal,
            invoiceTaxAmount,
            invoiceTotal,
            request.Notes,
            null, null, null, null,
            AuditField.Create()
        );

        foreach (var item in itemEntities)
            invoice.AddItem(item);

        _invoiceRepository.Add(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var includes = new List<Expression<Func<Invoice, object>>>
        {
            e => e.Client!,
            e => e.Currency,
            e => e.Items
        };

        var invoices = await _invoiceRepository.GetAsync(
            predicate: e => e.Id == invoiceId && e.AuditField.IsActive,
            includes: includes);

        return InvoiceMapper.MapToDto(invoices.First());
    }
}
