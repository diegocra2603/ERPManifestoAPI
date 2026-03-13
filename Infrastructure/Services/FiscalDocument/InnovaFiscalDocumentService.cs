using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;
using Domain.Contracts.Infrastructure.Persistence;
using Domain.Contracts.Infrastructure.Services.FiscalDocument;
using Domain.Entities.FiscalDocuments;
using Domain.Entities.SystemSettings;
using Domain.Primitives;
using Domain.ValueObjects;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FiscalDoc = Domain.Entities.FiscalDocuments.FiscalDocument;

namespace Services.FiscalDocument;

public class InnovaFiscalDocumentService : IFiscalDocumentService
{
    private readonly FiscalDocumentConfiguration _configuration;
    private readonly IAsyncRepository<FiscalDoc> _documentRepository;
    private readonly IAsyncRepository<FiscalDocumentItem> _itemRepository;
    private readonly IAsyncRepository<SystemSetting> _settingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<InnovaFiscalDocumentService> _logger;

    public InnovaFiscalDocumentService(
        IOptions<FiscalDocumentConfiguration> configuration,
        IAsyncRepository<FiscalDoc> documentRepository,
        IAsyncRepository<FiscalDocumentItem> itemRepository,
        IAsyncRepository<SystemSetting> settingRepository,
        IUnitOfWork unitOfWork,
        IHttpClientFactory httpClientFactory,
        ILogger<InnovaFiscalDocumentService> logger)
    {
        _configuration = configuration.Value;
        _documentRepository = documentRepository;
        _itemRepository = itemRepository;
        _settingRepository = settingRepository;
        _unitOfWork = unitOfWork;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<ErrorOr<FiscalDoc>> GenerateDocumentAsync(GenerateDocumentRequest request)
    {
        var settings = await LoadSettingsAsync();
        if (settings.IsError)
            return settings.Errors;

        var taxPercentage = decimal.Parse(settings.Value[SystemSettingKeys.TaxPercentage]) / 100m;
        var timezone = TimeZoneInfo.FindSystemTimeZoneById(settings.Value[SystemSettingKeys.Timezone]);
        var fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timezone);

        var exchangeRate = request.Moneda == CurrencyType.Dolar
            ? decimal.Parse(settings.Value[SystemSettingKeys.ExchangeRateUSD])
            : 1m;

        var calculatedItems = CalculateItems(request.Items, taxPercentage);

        var bruto = calculatedItems.Sum(i => i.GrossAmount);
        var descuento = calculatedItems.Sum(i => i.DiscountAmount);
        var exento = calculatedItems.Sum(i => i.ExemptAmount);
        var neto = calculatedItems.Sum(i => i.NetAmount);
        var iva = calculatedItems.Sum(i => i.IvaAmount);
        var isr = calculatedItems.Sum(i => i.IsrAmount);
        var total = calculatedItems.Sum(i => i.TotalAmount);

        var documentId = new FiscalDocumentId(Guid.NewGuid());

        var document = new FiscalDoc(
            id: documentId,
            documentType: request.DocumentType,
            status: FiscalDocumentStatus.Pending,
            nitReceptor: request.NitReceptor,
            nombreReceptor: request.NombreReceptor,
            direccionReceptor: request.DireccionReceptor,
            tipoVenta: request.TipoVenta,
            destinoVenta: request.DestinoVenta,
            fecha: fecha,
            moneda: request.Moneda,
            tasa: exchangeRate,
            referencia: request.Referencia,
            numeroAcceso: null,
            serieAdmin: request.SerieAdmin,
            numeroAdmin: request.NumeroAdmin,
            bruto: bruto,
            descuento: descuento,
            exento: exento,
            otros: 0,
            neto: neto,
            isr: isr,
            iva: iva,
            total: total,
            serie: null,
            preimpreso: null,
            numeroAutorizacion: null,
            docAsociadoSerie: request.DocAsociadoSerie,
            docAsociadoPreimpreso: request.DocAsociadoPreimpreso,
            errorMessage: null,
            xmlEnviado: null,
            auditField: AuditField.Create());

        foreach (var item in calculatedItems)
        {
            document.AddItem(new FiscalDocumentItem(
                id: new FiscalDocumentItemId(Guid.NewGuid()),
                fiscalDocumentId: documentId,
                productCode: item.ProductCode,
                description: item.Description,
                measureUnit: item.MeasureUnit,
                quantity: item.Quantity,
                price: item.Price,
                discountPercentage: item.DiscountPercentage,
                grossAmount: item.GrossAmount,
                discountAmount: item.DiscountAmount,
                exemptAmount: item.ExemptAmount,
                otherTaxes: item.OtherTaxes,
                netAmount: item.NetAmount,
                isrAmount: item.IsrAmount,
                ivaAmount: item.IvaAmount,
                totalAmount: item.TotalAmount,
                saleType: item.SaleType));
        }

        var xml = BuildDocumentXml(document, calculatedItems);
        document.SetXmlEnviado(xml);

        var result = await CallGeneraDocumentoAsync(request.DocumentType, xml);

        if (result.IsError)
        {
            var isConnectionError = result.Errors.Any(e =>
                e.Code == "FiscalDocument.ConnectionError");

            if (isConnectionError)
            {
                document.MarkAsContingency(GenerateLocalAccessNumber());
                _documentRepository.Add(document);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogWarning(
                    "Documento {Referencia} guardado en contingencia con NumeroAcceso {NumeroAcceso}",
                    document.Referencia, document.NumeroAcceso);

                return document;
            }

            document.MarkAsError(result.Errors.First().Description);
            _documentRepository.Add(document);
            await _unitOfWork.SaveChangesAsync();

            return result.Errors;
        }

        var (serie, preimpreso, numeroAutorizacion) = result.Value;
        document.MarkAsCertified(serie, preimpreso, numeroAutorizacion);

        _documentRepository.Add(document);
        await _unitOfWork.SaveChangesAsync();

        return document;
    }

    public async Task<ErrorOr<FiscalDoc>> VoidDocumentAsync(VoidDocumentRequest request)
    {
        var result = await CallAnulaDocumentoAsync(request);

        if (result.IsError)
            return result.Errors;

        var existingDoc = await _documentRepository.FirstOrDefaultAsync(
            d => d.Serie == request.Serie &&
                 d.Preimpreso == request.Preimpreso &&
                 d.AuditField.IsActive);

        if (existingDoc is not null)
        {
            existingDoc.MarkAsVoided();
            _documentRepository.Update(existingDoc);
            await _unitOfWork.SaveChangesAsync();
            return existingDoc;
        }

        return Error.NotFound(
            code: "FiscalDocument.NotFound",
            description: "No se encontró el documento fiscal en la base de datos local.");
    }

    public async Task<ErrorOr<List<long>>> GenerateContingencyNumbersAsync(int cantidad, string lote)
    {
        try
        {
            var client = CreateAuthenticatedClient();

            var url = $"{_configuration.ContingencyUrl}?" +
                      $"pUsuario={Uri.EscapeDataString(_configuration.User)}" +
                      $"&pPassword={Uri.EscapeDataString(_configuration.Password)}" +
                      $"&pNitEmisor={Uri.EscapeDataString(_configuration.Nit)}" +
                      $"&pEstablecimiento={_configuration.Establecimiento}" +
                      $"&pLote={Uri.EscapeDataString(lote)}" +
                      $"&pCantidad={cantidad}";

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return ParseContingencyResponse(responseContent);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de conexión al solicitar números de contingencia");
            return Error.Failure(
                code: "FiscalDocument.ContingencyConnectionError",
                description: $"Error de conexión al servicio de contingencia: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al solicitar números de contingencia");
            return Error.Failure(
                code: "FiscalDocument.ContingencyError",
                description: $"Error al solicitar números de contingencia: {ex.Message}");
        }
    }

    public async Task<ErrorOr<List<FiscalDoc>>> UploadContingencyDocumentsAsync()
    {
        var contingencyDocs = await _documentRepository.GetAsync(
            d => d.Status == FiscalDocumentStatus.Contingency && d.AuditField.IsActive);

        if (!contingencyDocs.Any())
        {
            return Error.NotFound(
                code: "FiscalDocument.NoContingencyDocuments",
                description: "No hay documentos en contingencia pendientes de envío.");
        }

        var uploadedDocs = new List<FiscalDoc>();

        foreach (var doc in contingencyDocs)
        {
            if (string.IsNullOrEmpty(doc.XmlEnviado))
                continue;

            var result = await CallGeneraDocumentoAsync(doc.DocumentType, doc.XmlEnviado);

            if (result.IsError)
            {
                _logger.LogWarning(
                    "No se pudo cargar documento en contingencia {Referencia}: {Error}",
                    doc.Referencia, result.Errors.First().Description);
                continue;
            }

            var (serie, preimpreso, numeroAutorizacion) = result.Value;
            doc.MarkAsCertified(serie, preimpreso, numeroAutorizacion);
            _documentRepository.Update(doc);
            uploadedDocs.Add(doc);
        }

        if (uploadedDocs.Any())
            await _unitOfWork.SaveChangesAsync();

        return uploadedDocs;
    }

    private async Task<ErrorOr<Dictionary<string, string>>> LoadSettingsAsync()
    {
        var settings = await _settingRepository.GetAsync(s => s.AuditField.IsActive);

        var dict = settings.ToDictionary(s => s.Key, s => s.Value);

        var requiredKeys = new[]
        {
            SystemSettingKeys.TaxPercentage,
            SystemSettingKeys.Timezone,
            SystemSettingKeys.ExchangeRateUSD
        };

        foreach (var key in requiredKeys)
        {
            if (!dict.ContainsKey(key))
            {
                return Error.Failure(
                    code: "FiscalDocument.MissingSetting",
                    description: $"Falta la configuración del sistema: {key}");
            }
        }

        return dict;
    }

    private static List<CalculatedItem> CalculateItems(
        List<GenerateDocumentItemRequest> items,
        decimal taxPercentage)
    {
        return items.Select(item =>
        {
            var grossAmount = Math.Round(item.Quantity * item.Price, 2);
            var discountAmount = Math.Round(grossAmount * (item.DiscountPercentage / 100m), 2);
            var subtotal = grossAmount - discountAmount;

            decimal exemptAmount = 0;
            decimal netAmount;
            decimal ivaAmount;

            if (item.IsExempt)
            {
                exemptAmount = subtotal;
                netAmount = 0;
                ivaAmount = 0;
            }
            else
            {
                // IVA incluido en el precio: Neto = Subtotal / (1 + taxPercentage)
                netAmount = Math.Round(subtotal / (1 + taxPercentage), 2);
                ivaAmount = subtotal - netAmount;
            }

            var totalAmount = subtotal;

            return new CalculatedItem(
                ProductCode: item.ProductCode,
                Description: item.Description,
                MeasureUnit: item.MeasureUnit,
                Quantity: item.Quantity,
                Price: item.Price,
                DiscountPercentage: item.DiscountPercentage,
                GrossAmount: grossAmount,
                DiscountAmount: discountAmount,
                ExemptAmount: exemptAmount,
                OtherTaxes: 0,
                NetAmount: netAmount,
                IsrAmount: 0,
                IvaAmount: ivaAmount,
                TotalAmount: totalAmount,
                SaleType: item.SaleType);
        }).ToList();
    }

    private async Task<ErrorOr<(string Serie, string Preimpreso, string NumeroAutorizacion)>>
        CallGeneraDocumentoAsync(FiscalDocumentType documentType, string xml)
    {
        try
        {
            var client = CreateAuthenticatedClient();

            var tipoDoc = (int)documentType;
            var url = $"{_configuration.Url}?" +
                      $"pUsuario={Uri.EscapeDataString(_configuration.User)}" +
                      $"&pPassword={Uri.EscapeDataString(_configuration.Password)}" +
                      $"&pNitEmisor={Uri.EscapeDataString(_configuration.Nit)}" +
                      $"&pEstablecimiento={_configuration.Establecimiento}" +
                      $"&pTipoDoc={tipoDoc}" +
                      $"&pIdMaquina={Uri.EscapeDataString(_configuration.IdMaquina)}" +
                      $"&pTipoRespuesta=D" +
                      $"&pXml={Uri.EscapeDataString(xml)}";

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return ParseGeneraDocumentoResponse(responseContent);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de conexión al generar documento fiscal");
            return Error.Failure(
                code: "FiscalDocument.ConnectionError",
                description: $"Error de conexión con el servicio de facturación: {ex.Message}");
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogError(ex, "Timeout al generar documento fiscal");
            return Error.Failure(
                code: "FiscalDocument.ConnectionError",
                description: "Timeout al conectar con el servicio de facturación.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al generar documento fiscal");
            return Error.Failure(
                code: "FiscalDocument.ServiceError",
                description: $"Error al generar documento fiscal: {ex.Message}");
        }
    }

    private async Task<ErrorOr<bool>> CallAnulaDocumentoAsync(VoidDocumentRequest request)
    {
        try
        {
            var client = CreateAuthenticatedClient();

            var url = $"{_configuration.Url}?" +
                      $"pUsuario={Uri.EscapeDataString(_configuration.User)}" +
                      $"&pPassword={Uri.EscapeDataString(_configuration.Password)}" +
                      $"&pNitEmisor={Uri.EscapeDataString(_configuration.Nit)}" +
                      $"&pSerie={Uri.EscapeDataString(request.Serie)}" +
                      $"&pPreimpreso={Uri.EscapeDataString(request.Preimpreso)}" +
                      $"&pNitComprador={Uri.EscapeDataString(request.NitComprador)}" +
                      $"&pFechaAnulacion={Uri.EscapeDataString(request.FechaAnulacion)}" +
                      $"&pMotivoAnulacion={Uri.EscapeDataString(request.MotivoAnulacion)}";

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return ParseAnulaDocumentoResponse(responseContent);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de conexión al anular documento fiscal");
            return Error.Failure(
                code: "FiscalDocument.ConnectionError",
                description: $"Error de conexión al anular documento: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al anular documento fiscal");
            return Error.Failure(
                code: "FiscalDocument.VoidError",
                description: $"Error al anular documento fiscal: {ex.Message}");
        }
    }

    private HttpClient CreateAuthenticatedClient()
    {
        var client = _httpClientFactory.CreateClient();
        var credentials = Convert.ToBase64String(
            Encoding.ASCII.GetBytes($"{_configuration.UserHeader}:{_configuration.PasswordHeader}"));
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", credentials);
        client.Timeout = TimeSpan.FromSeconds(30);
        return client;
    }

    private string BuildDocumentXml(FiscalDoc document, List<CalculatedItem> items)
    {
        var sb = new StringBuilder();
        sb.Append("<DocElectronico>");
        sb.Append("<Encabezado>");

        sb.Append("<Receptor>");
        sb.Append($"<NITReceptor>{EscapeXml(document.NitReceptor)}</NITReceptor>");
        if (!string.IsNullOrEmpty(document.NombreReceptor))
            sb.Append($"<Nombre>{EscapeXml(document.NombreReceptor)}</Nombre>");
        if (!string.IsNullOrEmpty(document.DireccionReceptor))
            sb.Append($"<Direccion>{EscapeXml(document.DireccionReceptor)}</Direccion>");
        sb.Append("</Receptor>");

        sb.Append("<InfoDoc>");
        sb.Append($"<TipoVenta>{document.TipoVenta.ToCode()}</TipoVenta>");
        sb.Append($"<DestinoVenta>{(int)document.DestinoVenta}</DestinoVenta>");
        sb.Append($"<Fecha>{document.Fecha:dd/MM/yyyy}</Fecha>");
        sb.Append($"<Moneda>{(int)document.Moneda}</Moneda>");
        sb.Append($"<Tasa>{document.Tasa}</Tasa>");
        sb.Append($"<Referencia>{EscapeXml(document.Referencia)}</Referencia>");
        if (document.SerieAdmin is not null)
            sb.Append($"<SerieAdmin>{EscapeXml(document.SerieAdmin)}</SerieAdmin>");
        if (document.NumeroAdmin is not null)
            sb.Append($"<NumeroAdmin>{document.NumeroAdmin}</NumeroAdmin>");
        sb.Append("</InfoDoc>");

        sb.Append("<Totales>");
        sb.Append($"<Bruto>{document.Bruto}</Bruto>");
        sb.Append($"<Descuento>{document.Descuento}</Descuento>");
        sb.Append($"<Exento>{document.Exento}</Exento>");
        sb.Append($"<Otros>{document.Otros}</Otros>");
        sb.Append($"<Neto>{document.Neto}</Neto>");
        sb.Append($"<Isr>{document.Isr}</Isr>");
        sb.Append($"<Iva>{document.Iva}</Iva>");
        sb.Append($"<Total>{document.Total}</Total>");
        sb.Append("</Totales>");

        sb.Append("</Encabezado>");

        sb.Append("<Detalles>");
        foreach (var item in items)
        {
            sb.Append("<Productos>");
            sb.Append($"<Producto>{EscapeXml(item.ProductCode)}</Producto>");
            sb.Append($"<Descripcion>{EscapeXml(item.Description)}</Descripcion>");
            sb.Append($"<Medida>{item.MeasureUnit}</Medida>");
            sb.Append($"<Cantidad>{item.Quantity}</Cantidad>");
            sb.Append($"<Precio>{item.Price}</Precio>");
            sb.Append($"<PorcDesc>{item.DiscountPercentage}</PorcDesc>");
            sb.Append($"<ImpBruto>{item.GrossAmount}</ImpBruto>");
            sb.Append($"<ImpDescuento>{item.DiscountAmount}</ImpDescuento>");
            sb.Append($"<ImpExento>{item.ExemptAmount}</ImpExento>");
            sb.Append($"<ImpOtros>{item.OtherTaxes}</ImpOtros>");
            sb.Append($"<ImpNeto>{item.NetAmount}</ImpNeto>");
            sb.Append($"<ImpIsr>{item.IsrAmount}</ImpIsr>");
            sb.Append($"<ImpIva>{item.IvaAmount}</ImpIva>");
            sb.Append($"<ImpTotal>{item.TotalAmount}</ImpTotal>");
            sb.Append($"<TipoVentaDet>{item.SaleType.ToCode()}</TipoVentaDet>");
            sb.Append("</Productos>");
        }

        if (!string.IsNullOrEmpty(document.DocAsociadoSerie) &&
            !string.IsNullOrEmpty(document.DocAsociadoPreimpreso))
        {
            sb.Append("<DocAsociados>");
            sb.Append($"<DASerie>{EscapeXml(document.DocAsociadoSerie)}</DASerie>");
            sb.Append($"<DAPreimpreso>{EscapeXml(document.DocAsociadoPreimpreso)}</DAPreimpreso>");
            sb.Append("</DocAsociados>");
        }

        sb.Append("</Detalles>");
        sb.Append("</DocElectronico>");

        return sb.ToString();
    }

    private static ErrorOr<(string Serie, string Preimpreso, string NumeroAutorizacion)>
        ParseGeneraDocumentoResponse(string responseContent)
    {
        try
        {
            var xmlContent = ExtractReturnContent(responseContent);

            var doc = XDocument.Parse($"<root>{xmlContent}</root>");
            var resultado = doc.Descendants("Resultado").FirstOrDefault();

            if (resultado is null)
            {
                return Error.Failure(
                    code: "FiscalDocument.InvalidResponse",
                    description: "La respuesta del servicio no tiene el formato esperado.");
            }

            var serie = resultado.Element("Serie")?.Value;
            var preimpreso = resultado.Element("Preimpreso")?.Value;
            var numeroAutorizacion = resultado.Element("NumeroAutorizacion")?.Value;

            if (serie is not null && preimpreso is not null)
            {
                return (serie, preimpreso.Trim(), numeroAutorizacion ?? "");
            }

            var errorMessage = resultado.Value;
            return Error.Failure(
                code: "FiscalDocument.GenerationFailed",
                description: $"Error al generar documento: {errorMessage}");
        }
        catch
        {
            return Error.Failure(
                code: "FiscalDocument.ParseError",
                description: "No se pudo interpretar la respuesta del servicio de facturación.");
        }
    }

    private static ErrorOr<bool> ParseAnulaDocumentoResponse(string responseContent)
    {
        try
        {
            var xmlContent = ExtractReturnContent(responseContent);

            var doc = XDocument.Parse($"<root>{xmlContent}</root>");
            var resultado = doc.Descendants("RESULTADO").FirstOrDefault();

            if (resultado is null)
            {
                return Error.Failure(
                    code: "FiscalDocument.InvalidResponse",
                    description: "La respuesta del servicio de anulación no tiene el formato esperado.");
            }

            var estado = resultado.Element("ESTADO")?.Value;

            if (estado == "ANULADO")
                return true;

            return Error.Failure(
                code: "FiscalDocument.VoidFailed",
                description: $"No se pudo anular el documento: {resultado.Value}");
        }
        catch
        {
            return Error.Failure(
                code: "FiscalDocument.ParseError",
                description: "No se pudo interpretar la respuesta del servicio de anulación.");
        }
    }

    private static ErrorOr<List<long>> ParseContingencyResponse(string responseContent)
    {
        try
        {
            var xmlContent = ExtractReturnContent(responseContent);

            var doc = XDocument.Parse($"<root>{xmlContent}</root>");
            var resultado = doc.Descendants("Resultado").FirstOrDefault();

            if (resultado is not null && !resultado.Elements("NumeroAcceso").Any())
            {
                return Error.Failure(
                    code: "FiscalDocument.ContingencyFailed",
                    description: $"Error en contingencia: {resultado.Value}");
            }

            var numeros = doc.Descendants("NumeroAcceso")
                .Select(n => long.Parse(n.Value))
                .ToList();

            if (numeros.Count == 0)
            {
                return Error.Failure(
                    code: "FiscalDocument.NoContingencyNumbers",
                    description: "No se recibieron números de contingencia.");
            }

            return numeros;
        }
        catch
        {
            return Error.Failure(
                code: "FiscalDocument.ParseError",
                description: "No se pudo interpretar la respuesta del servicio de contingencia.");
        }
    }

    private static string ExtractReturnContent(string soapResponse)
    {
        var returnStart = soapResponse.IndexOf("<return", StringComparison.Ordinal);
        if (returnStart < 0) return soapResponse;

        var contentStart = soapResponse.IndexOf('>', returnStart) + 1;
        var contentEnd = soapResponse.IndexOf("</return>", StringComparison.Ordinal);

        if (contentStart <= 0 || contentEnd < 0) return soapResponse;

        return soapResponse[contentStart..contentEnd];
    }

    private static long GenerateLocalAccessNumber()
    {
        var random = new Random();
        return random.NextInt64(100000000, 999999999);
    }

    private static string EscapeXml(string value)
    {
        return value
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
    }

    private record CalculatedItem(
        string ProductCode,
        string Description,
        int MeasureUnit,
        decimal Quantity,
        decimal Price,
        decimal DiscountPercentage,
        decimal GrossAmount,
        decimal DiscountAmount,
        decimal ExemptAmount,
        decimal OtherTaxes,
        decimal NetAmount,
        decimal IsrAmount,
        decimal IvaAmount,
        decimal TotalAmount,
        SaleType SaleType);
}
