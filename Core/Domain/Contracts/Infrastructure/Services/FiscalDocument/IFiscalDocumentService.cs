using Domain.Entities.FiscalDocuments;
using ErrorOr;

namespace Domain.Contracts.Infrastructure.Services.FiscalDocument;

public interface IFiscalDocumentService
{
    Task<ErrorOr<Entities.FiscalDocuments.FiscalDocument>> GenerateDocumentAsync(GenerateDocumentRequest request);
    Task<ErrorOr<Entities.FiscalDocuments.FiscalDocument>> VoidDocumentAsync(VoidDocumentRequest request);
    Task<ErrorOr<List<long>>> GenerateContingencyNumbersAsync(int cantidad, string lote);
    Task<ErrorOr<List<Entities.FiscalDocuments.FiscalDocument>>> UploadContingencyDocumentsAsync();
}

public record GenerateDocumentRequest(
    FiscalDocumentType DocumentType,
    string NitReceptor,
    string? NombreReceptor,
    string? DireccionReceptor,
    SaleType TipoVenta,
    DestinationType DestinoVenta,
    CurrencyType Moneda,
    string Referencia,
    string? SerieAdmin,
    long? NumeroAdmin,
    string? DocAsociadoSerie,
    string? DocAsociadoPreimpreso,
    List<GenerateDocumentItemRequest> Items);

public record GenerateDocumentItemRequest(
    string ProductCode,
    string Description,
    int MeasureUnit,
    decimal Quantity,
    decimal Price,
    decimal DiscountPercentage,
    SaleType SaleType,
    bool IsExempt = false);

public record VoidDocumentRequest(
    string Serie,
    string Preimpreso,
    string NitComprador,
    string FechaAnulacion,
    string MotivoAnulacion);
