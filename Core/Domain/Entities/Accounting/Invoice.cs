using Domain.Entities.Accounting.Enums;
using Domain.Primitives;
using Domain.ValueObjects;

namespace Domain.Entities.Accounting;

public sealed class Invoice : AggregateRoot
{
    private readonly List<InvoiceItem> _items = new();
    private Invoice() { }

    public Invoice(
        InvoiceId id,
        InvoiceType invoiceType,
        InvoiceStatus status,
        string invoiceNumber,
        DateTime date,
        DateTime? dueDate,
        ClientId? clientId,
        SupplierId? supplierId,
        string nit,
        string name,
        string? address,
        CurrencyId currencyId,
        decimal exchangeRate,
        decimal subtotal,
        decimal taxAmount,
        decimal total,
        string? notes,
        JournalEntryId? journalEntryId,
        string? fiscalSerie,
        string? fiscalNumero,
        string? fiscalAutorizacion,
        AuditField auditField)
    {
        Id = id;
        InvoiceType = invoiceType;
        Status = status;
        InvoiceNumber = invoiceNumber;
        Date = date;
        DueDate = dueDate;
        ClientId = clientId;
        SupplierId = supplierId;
        Nit = nit;
        Name = name;
        Address = address;
        CurrencyId = currencyId;
        ExchangeRate = exchangeRate;
        Subtotal = subtotal;
        TaxAmount = taxAmount;
        Total = total;
        Notes = notes;
        JournalEntryId = journalEntryId;
        FiscalSerie = fiscalSerie;
        FiscalNumero = fiscalNumero;
        FiscalAutorizacion = fiscalAutorizacion;
        AuditField = auditField;
    }

    public InvoiceId Id { get; private set; } = default!;
    public InvoiceType InvoiceType { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public string InvoiceNumber { get; private set; } = default!;
    public DateTime Date { get; private set; }
    public DateTime? DueDate { get; private set; }
    public ClientId? ClientId { get; private set; }
    public SupplierId? SupplierId { get; private set; }
    public string Nit { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string? Address { get; private set; }
    public CurrencyId CurrencyId { get; private set; } = default!;
    public decimal ExchangeRate { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal Total { get; private set; }
    public string? Notes { get; private set; }
    public JournalEntryId? JournalEntryId { get; private set; }
    public string? FiscalSerie { get; private set; }
    public string? FiscalNumero { get; private set; }
    public string? FiscalAutorizacion { get; private set; }
    public AuditField AuditField { get; private set; } = default!;

    // Navigation
    public Client? Client { get; private set; }
    public Supplier? Supplier { get; private set; }
    public Currency Currency { get; private set; } = default!;
    public JournalEntry? JournalEntry { get; private set; }
    public IReadOnlyCollection<InvoiceItem> Items => _items.AsReadOnly();

    public void AddItem(InvoiceItem item) { _items.Add(item); }

    public void SetFiscalData(string? serie, string? numero, string? autorizacion)
    {
        FiscalSerie = serie;
        FiscalNumero = numero;
        FiscalAutorizacion = autorizacion;
        AuditField = AuditField.Update();
    }

    public void SetJournalEntry(JournalEntryId journalEntryId)
    {
        JournalEntryId = journalEntryId;
        AuditField = AuditField.Update();
    }

    public void Emit()
    {
        Status = InvoiceStatus.Emitida;
        AuditField = AuditField.Update();
    }

    public void MarkAsPaid()
    {
        Status = InvoiceStatus.Pagada;
        AuditField = AuditField.Update();
    }

    public void Void()
    {
        Status = InvoiceStatus.Anulada;
        AuditField = AuditField.Update();
    }

    public void MarkAsDeleted()
    {
        AuditField = AuditField.MarkDeleted();
    }
}
