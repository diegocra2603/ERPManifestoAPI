using Domain.Primitives;
using Domain.ValueObjects;

namespace Domain.Entities.FiscalDocuments;

public sealed class FiscalDocument : AggregateRoot
{
    private readonly List<FiscalDocumentItem> _items = new();

    private FiscalDocument() { }

    public FiscalDocument(
        FiscalDocumentId id,
        FiscalDocumentType documentType,
        FiscalDocumentStatus status,
        string nitReceptor,
        string? nombreReceptor,
        string? direccionReceptor,
        SaleType tipoVenta,
        DestinationType destinoVenta,
        DateTime fecha,
        CurrencyType moneda,
        decimal tasa,
        string referencia,
        long? numeroAcceso,
        string? serieAdmin,
        long? numeroAdmin,
        decimal bruto,
        decimal descuento,
        decimal exento,
        decimal otros,
        decimal neto,
        decimal isr,
        decimal iva,
        decimal total,
        string? serie,
        string? preimpreso,
        string? numeroAutorizacion,
        string? docAsociadoSerie,
        string? docAsociadoPreimpreso,
        string? errorMessage,
        string? xmlEnviado,
        AuditField auditField)
    {
        Id = id;
        DocumentType = documentType;
        Status = status;
        NitReceptor = nitReceptor;
        NombreReceptor = nombreReceptor;
        DireccionReceptor = direccionReceptor;
        TipoVenta = tipoVenta;
        DestinoVenta = destinoVenta;
        Fecha = fecha;
        Moneda = moneda;
        Tasa = tasa;
        Referencia = referencia;
        NumeroAcceso = numeroAcceso;
        SerieAdmin = serieAdmin;
        NumeroAdmin = numeroAdmin;
        Bruto = bruto;
        Descuento = descuento;
        Exento = exento;
        Otros = otros;
        Neto = neto;
        Isr = isr;
        Iva = iva;
        Total = total;
        Serie = serie;
        Preimpreso = preimpreso;
        NumeroAutorizacion = numeroAutorizacion;
        DocAsociadoSerie = docAsociadoSerie;
        DocAsociadoPreimpreso = docAsociadoPreimpreso;
        ErrorMessage = errorMessage;
        XmlEnviado = xmlEnviado;
        AuditField = auditField;
    }

    public FiscalDocumentId Id { get; private set; } = default!;
    public FiscalDocumentType DocumentType { get; private set; }
    public FiscalDocumentStatus Status { get; private set; }

    // Receptor
    public string NitReceptor { get; private set; } = default!;
    public string? NombreReceptor { get; private set; }
    public string? DireccionReceptor { get; private set; }

    // InfoDoc
    public SaleType TipoVenta { get; private set; }
    public DestinationType DestinoVenta { get; private set; }
    public DateTime Fecha { get; private set; }
    public CurrencyType Moneda { get; private set; }
    public decimal Tasa { get; private set; }
    public string Referencia { get; private set; } = default!;
    public long? NumeroAcceso { get; private set; }
    public string? SerieAdmin { get; private set; }
    public long? NumeroAdmin { get; private set; }

    // Totales
    public decimal Bruto { get; private set; }
    public decimal Descuento { get; private set; }
    public decimal Exento { get; private set; }
    public decimal Otros { get; private set; }
    public decimal Neto { get; private set; }
    public decimal Isr { get; private set; }
    public decimal Iva { get; private set; }
    public decimal Total { get; private set; }

    // Respuesta Ainnova
    public string? Serie { get; private set; }
    public string? Preimpreso { get; private set; }
    public string? NumeroAutorizacion { get; private set; }

    // Documentos Asociados (Notas de crédito/débito)
    public string? DocAsociadoSerie { get; private set; }
    public string? DocAsociadoPreimpreso { get; private set; }

    // Error / XML
    public string? ErrorMessage { get; private set; }
    public string? XmlEnviado { get; private set; }

    public AuditField AuditField { get; private set; } = default!;
    public IReadOnlyCollection<FiscalDocumentItem> Items => _items.AsReadOnly();

    public void AddItem(FiscalDocumentItem item)
    {
        _items.Add(item);
    }

    public void MarkAsCertified(string serie, string preimpreso, string numeroAutorizacion)
    {
        Status = FiscalDocumentStatus.Certified;
        Serie = serie;
        Preimpreso = preimpreso;
        NumeroAutorizacion = numeroAutorizacion;
        AuditField = AuditField.Update();
    }

    public void MarkAsContingency(long numeroAcceso)
    {
        Status = FiscalDocumentStatus.Contingency;
        NumeroAcceso = numeroAcceso;
        AuditField = AuditField.Update();
    }

    public void MarkAsVoided()
    {
        Status = FiscalDocumentStatus.Voided;
        AuditField = AuditField.Update();
    }

    public void MarkAsError(string errorMessage)
    {
        Status = FiscalDocumentStatus.Error;
        ErrorMessage = errorMessage;
        AuditField = AuditField.Update();
    }

    public void SetXmlEnviado(string xml)
    {
        XmlEnviado = xml;
    }
}
