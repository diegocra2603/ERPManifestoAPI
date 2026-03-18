using System.Linq.Expressions;
using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Entities.Accounting.Enums;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using Domain.ValueObjects;
using ErrorOr;

namespace Application.Features.Invoices.CreateCreditNote;

public class CreateCreditNoteCommandHandler : IRequestHandler<CreateCreditNoteCommand, ErrorOr<InvoiceDto>>
{
    private readonly IAsyncRepository<Invoice> _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCreditNoteCommandHandler(
        IAsyncRepository<Invoice> invoiceRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
    }

    private static DateTime ToUtc(DateTime dt) =>
        dt.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(dt, DateTimeKind.Utc) : dt.ToUniversalTime();

    public async Task<ErrorOr<InvoiceDto>> Handle(CreateCreditNoteCommand request, CancellationToken cancellationToken)
    {
        // Load original invoice
        var includes = new List<Expression<Func<Invoice, object>>>
        {
            e => e.Items,
            e => e.Client!,
            e => e.Currency
        };

        var originals = await _invoiceRepository.GetAsync(
            predicate: i => i.Id == new InvoiceId(request.OriginalInvoiceId) && i.AuditField.IsActive,
            includes: includes);

        var original = originals.FirstOrDefault();

        if (original is null)
            return Error.NotFound("Invoice.NotFound", "Factura original no encontrada.");

        if (original.Status != InvoiceStatus.Emitida)
            return Error.Validation("Invoice.NotEmitted", "Solo se pueden crear notas de crédito sobre facturas emitidas.");

        if (string.IsNullOrWhiteSpace(original.FiscalSerie) || original.FiscalSerie.StartsWith("CONTINGENCIA"))
            return Error.Validation("Invoice.NoFiscalData", "La factura original debe estar certificada para crear una nota de crédito.");

        // Validate credit note items don't exceed original amounts
        foreach (var item in request.Items)
        {
            if (item.Quantity <= 0 || item.UnitPrice <= 0)
                return Error.Validation("Invoice.InvalidItem", "La cantidad y el precio deben ser mayores a 0.");
        }

        var creditNoteTotal = request.Items.Sum(i => i.Quantity * i.UnitPrice * 1.12m);
        if (creditNoteTotal > original.Total)
            return Error.Validation("Invoice.ExceedsOriginal",
                $"El total de la nota de crédito (Q{creditNoteTotal:N2}) no puede exceder el total de la factura original (Q{original.Total:N2}).");

        // Generate credit note number
        var existingCreditNotes = await _invoiceRepository.GetAsync(
            i => i.InvoiceType == InvoiceType.CreditNote && i.AuditField.IsActive);
        var creditNoteNumber = "NC-" + (existingCreditNotes.Count + 1).ToString("D6");

        var creditNoteId = new InvoiceId(Guid.NewGuid());

        // Calculate items
        decimal subtotal = 0;
        decimal taxAmount = 0;
        decimal total = 0;
        var itemEntities = new List<InvoiceItem>();

        for (var i = 0; i < request.Items.Count; i++)
        {
            var itemDto = request.Items[i];
            var itemSubtotal = itemDto.Quantity * itemDto.UnitPrice;
            var itemTax = Math.Round(itemSubtotal * 0.12m, 2);
            var itemTotal = itemSubtotal + itemTax;

            subtotal += itemSubtotal;
            taxAmount += itemTax;
            total += itemTotal;

            itemEntities.Add(new InvoiceItem(
                new InvoiceItemId(Guid.NewGuid()),
                creditNoteId,
                itemDto.Description,
                itemDto.Quantity,
                itemDto.UnitPrice,
                itemSubtotal, itemTax, itemTotal,
                i + 1
            ));
        }

        // Build notes with associated invoice info
        var associatedInfo = $"Factura Asociada Serie {original.FiscalSerie} Numero {original.FiscalNumero} Fecha {original.Date:dd/MM/yyyy}";
        var notes = string.IsNullOrWhiteSpace(request.Notes)
            ? associatedInfo
            : $"{request.Notes}\n{associatedInfo}";

        var creditNote = new Invoice(
            creditNoteId,
            InvoiceType.CreditNote,
            InvoiceStatus.Borrador,
            creditNoteNumber,
            ToUtc(request.Date),
            null,
            original.ClientId,
            null,
            original.Nit,
            original.Name,
            original.Address,
            original.CurrencyId,
            original.ExchangeRate,
            subtotal,
            taxAmount,
            total,
            notes,
            null, null, null, null,
            AuditField.Create(),
            new InvoiceId(request.OriginalInvoiceId)
        );

        foreach (var item in itemEntities)
            creditNote.AddItem(item);

        _invoiceRepository.Add(creditNote);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload with includes
        var reloadIncludes = new List<Expression<Func<Invoice, object>>>
        {
            e => e.Client!,
            e => e.Currency,
            e => e.Items,
            e => e.OriginalInvoice!
        };

        var loaded = await _invoiceRepository.GetAsync(
            predicate: e => e.Id == creditNoteId && e.AuditField.IsActive,
            includes: reloadIncludes);

        return InvoiceMapper.MapToDto(loaded.First());
    }
}
