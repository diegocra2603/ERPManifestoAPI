using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using Domain.Contracts.Infrastructure.Persistence;
using Domain.Contracts.Infrastructure.Services.FiscalDocument;
using Domain.Entities.FiscalDocuments;
using Domain.Primitives;
using Domain.ValueObjects;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FiscalDoc = Domain.Entities.FiscalDocuments.FiscalDocument;

namespace Services.FiscalDocument;

public class InnovaFiscalDocumentService : IFiscalDocumentService
{
    private static readonly TimeZoneInfo GuatemalaTimezone = TimeZoneInfo.FindSystemTimeZoneById("America/Guatemala");

    private readonly FiscalDocumentConfiguration _configuration;
    private readonly IAsyncRepository<FiscalDoc> _documentRepository;
    private readonly IAsyncRepository<FiscalDocumentItem> _itemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<InnovaFiscalDocumentService> _logger;

    public InnovaFiscalDocumentService(
        IOptions<FiscalDocumentConfiguration> configuration,
        IAsyncRepository<FiscalDoc> documentRepository,
        IAsyncRepository<FiscalDocumentItem> itemRepository,
        IUnitOfWork unitOfWork,
        IHttpClientFactory httpClientFactory,
        ILogger<InnovaFiscalDocumentService> logger)
    {
        _configuration = configuration.Value;
        _documentRepository = documentRepository;
        _itemRepository = itemRepository;
        _unitOfWork = unitOfWork;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<ErrorOr<FiscalDoc>> GenerateDocumentAsync(GenerateDocumentRequest request)
    {
        var taxPercentage = request.TaxPercentage / 100m;
        var fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, GuatemalaTimezone);
        var exchangeRate = request.ExchangeRate;

        // 1. Calculate each item first
        var calculatedItems = CalculateItems(request.Items, taxPercentage);

        // 2. Totals are sum of items
        var bruto = calculatedItems.Sum(i => i.GrossAmount);
        var descuento = calculatedItems.Sum(i => i.DiscountAmount);
        var exento = calculatedItems.Sum(i => i.ExemptAmount);
        var neto = calculatedItems.Sum(i => i.NetAmount);
        var iva = calculatedItems.Sum(i => i.IvaAmount);
        var isr = calculatedItems.Sum(i => i.IsrAmount);
        var total = calculatedItems.Sum(i => i.TotalAmount);

        var documentId = new FiscalDocumentId(Guid.NewGuid());
        var numeroAdmin = new Random().NextInt64(100000000, 999999999);

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
            serieAdmin: request.SerieAdmin ?? "A",
            numeroAdmin: request.NumeroAdmin ?? numeroAdmin,
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
                // Rebuild XML with NumeroAcceso included for contingency upload
                var contingencyXml = BuildDocumentXml(document, calculatedItems);
                document.SetXmlEnviado(contingencyXml);
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

            var soapBody = $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ns1=""http://dbguatefac/Guatefac.wsdl"">
    <soapenv:Header/>
    <soapenv:Body>
        <ns1:generaNumerosContingencia>
            <pUsuario>{EscapeXml(_configuration.User)}</pUsuario>
            <pPassword>{EscapeXml(_configuration.Password)}</pPassword>
            <pNitEmisor>{EscapeXml(_configuration.Nit)}</pNitEmisor>
            <pEstablecimiento>{_configuration.Establecimiento}</pEstablecimiento>
            <pLote>{EscapeXml(lote)}</pLote>
            <pCantidad>{cantidad}</pCantidad>
        </ns1:generaNumerosContingencia>
    </soapenv:Body>
</soapenv:Envelope>";

            var content = new StringContent(soapBody, Encoding.UTF8, "text/xml");
            client.DefaultRequestHeaders.Add("SOAPAction", "http://dbguatefac/Guatefac.wsdl#generaNumerosContingencia");

            var response = await client.PostAsync(_configuration.ContingencyUrl ?? _configuration.Url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return Error.Failure(
                    code: "FiscalDocument.ContingencyConnectionError",
                    description: $"Error del servicio de contingencia (HTTP {(int)response.StatusCode})");
            }

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

    // ==================== SOAP CALLS ====================

    /// <summary>
    /// SOAP POST to Ainnova generaDocumento (same as Odoo fiscal_module_gt).
    /// </summary>
    private async Task<ErrorOr<(string Serie, string Preimpreso, string NumeroAutorizacion)>>
        CallGeneraDocumentoAsync(FiscalDocumentType documentType, string xml)
    {
        HttpResponseMessage? response = null;
        try
        {
            var client = CreateAuthenticatedClient();
            var tipoDoc = (int)documentType;

            var soapBody = $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ns1=""http://dbguatefac/Guatefac.wsdl"">
    <soapenv:Header/>
    <soapenv:Body>
        <ns1:generaDocumento>
            <pUsuario>{EscapeXml(_configuration.User)}</pUsuario>
            <pPassword>{EscapeXml(_configuration.Password)}</pPassword>
            <pNitEmisor>{EscapeXml(_configuration.Nit)}</pNitEmisor>
            <pEstablecimiento>{_configuration.Establecimiento}</pEstablecimiento>
            <pTipoDoc>{tipoDoc}</pTipoDoc>
            <pIdMaquina>{EscapeXml(_configuration.IdMaquina)}</pIdMaquina>
            <pTipoRespuesta>R</pTipoRespuesta>
            <pXml><![CDATA[{xml}]]></pXml>
        </ns1:generaDocumento>
    </soapenv:Body>
</soapenv:Envelope>";

            _logger.LogInformation(
                "Enviando SOAP a Ainnova: TipoDoc={TipoDoc}, URL={Url}, pNit={Nit}",
                tipoDoc, _configuration.Url, _configuration.Nit);
            _logger.LogInformation("XML DocElectronico:\n{Xml}", xml);

            var content = new StringContent(soapBody, Encoding.UTF8, "text/xml");
            client.DefaultRequestHeaders.Add("SOAPAction", "http://dbguatefac/Guatefac.wsdl#generaDocumento");

            response = await client.PostAsync(_configuration.Url, content);

            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Respuesta Ainnova: StatusCode={StatusCode}, Body={Body}",
                response.StatusCode, responseContent);

            if (!response.IsSuccessStatusCode)
            {
                return Error.Failure(
                    code: "FiscalDocument.ServiceError",
                    description: $"Error del servicio fiscal (HTTP {(int)response.StatusCode}): {responseContent}");
            }

            // Check for ERROR in response (same as Odoo parser)
            if (responseContent.Contains("ERROR", StringComparison.OrdinalIgnoreCase))
            {
                var errorMsg = ExtractSoapResultText(responseContent);
                return Error.Failure(
                    code: "FiscalDocument.GenerationFailed",
                    description: $"Error de Ainnova: {errorMsg}");
            }

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
            var body = response != null ? await response.Content.ReadAsStringAsync() : "Sin respuesta";
            _logger.LogError(ex, "Error inesperado al generar documento fiscal. Respuesta: {Body}", body);
            return Error.Failure(
                code: "FiscalDocument.ServiceError",
                description: $"Error al generar documento fiscal: {ex.Message}");
        }
    }

    /// <summary>
    /// SOAP POST to Ainnova anulaDocumento.
    /// </summary>
    private async Task<ErrorOr<bool>> CallAnulaDocumentoAsync(VoidDocumentRequest request)
    {
        HttpResponseMessage? response = null;
        try
        {
            var client = CreateAuthenticatedClient();

            var soapBody = $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ns1=""http://dbguatefac/Guatefac.wsdl"">
    <soapenv:Header/>
    <soapenv:Body>
        <ns1:anulaDocumento>
            <pUsuario>{EscapeXml(_configuration.User)}</pUsuario>
            <pPassword>{EscapeXml(_configuration.Password)}</pPassword>
            <pNitEmisor>{EscapeXml(_configuration.Nit)}</pNitEmisor>
            <pSerie>{EscapeXml(request.Serie)}</pSerie>
            <pPreimpreso>{EscapeXml(request.Preimpreso)}</pPreimpreso>
            <pNitComprador>{EscapeXml(request.NitComprador)}</pNitComprador>
            <pFechaAnulacion>{EscapeXml(request.FechaAnulacion)}</pFechaAnulacion>
            <pMotivoAnulacion>{EscapeXml(request.MotivoAnulacion)}</pMotivoAnulacion>
        </ns1:anulaDocumento>
    </soapenv:Body>
</soapenv:Envelope>";

            var content = new StringContent(soapBody, Encoding.UTF8, "text/xml");
            client.DefaultRequestHeaders.Add("SOAPAction", "http://dbguatefac/Guatefac.wsdl#anulaDocumento");

            response = await client.PostAsync(_configuration.Url, content);

            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Respuesta Ainnova Anulación: StatusCode={StatusCode}, Body={Body}",
                response.StatusCode, responseContent);

            if (!response.IsSuccessStatusCode)
            {
                return Error.Failure(
                    code: "FiscalDocument.ServiceError",
                    description: $"Error del servicio fiscal (HTTP {(int)response.StatusCode})");
            }

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

    // ==================== HTTP CLIENT ====================

    private HttpClient CreateAuthenticatedClient()
    {
        var client = _httpClientFactory.CreateClient();
        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{_configuration.UserHeader}:{_configuration.PasswordHeader}"));
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", credentials);
        client.Timeout = TimeSpan.FromSeconds(30);
        return client;
    }

    // ==================== XML BUILDING ====================

    private static string F2(decimal value) => value.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);

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
        sb.Append($"<Tasa>{F2(document.Tasa)}</Tasa>");
        sb.Append($"<Referencia>{EscapeXml(document.Referencia)}</Referencia>");
        if (document.NumeroAcceso.HasValue && document.NumeroAcceso > 0)
            sb.Append($"<NumeroAcceso>{document.NumeroAcceso}</NumeroAcceso>");
        sb.Append($"<SerieAdmin>{EscapeXml(document.SerieAdmin ?? "A")}</SerieAdmin>");
        sb.Append($"<NumeroAdmin>{document.NumeroAdmin}</NumeroAdmin>");
        sb.Append("<Reversion>N</Reversion>");
        sb.Append("</InfoDoc>");

        sb.Append("<Totales>");
        sb.Append($"<Bruto>{F2(document.Bruto)}</Bruto>");
        sb.Append($"<Descuento>{F2(document.Descuento)}</Descuento>");
        sb.Append($"<Exento>{F2(document.Exento)}</Exento>");
        sb.Append($"<Otros>{F2(document.Otros)}</Otros>");
        sb.Append($"<Neto>{F2(document.Neto)}</Neto>");
        sb.Append($"<Isr>{F2(document.Isr)}</Isr>");
        sb.Append($"<Iva>{F2(document.Iva)}</Iva>");
        sb.Append($"<Total>{F2(document.Total)}</Total>");
        sb.Append("</Totales>");

        sb.Append("</Encabezado>");

        sb.Append("<Detalles>");
        var lineIndex = 1;
        foreach (var item in items)
        {
            sb.Append("<Productos>");
            sb.Append($"<Producto>{lineIndex}00</Producto>");
            sb.Append($"<Descripcion>{EscapeXml(item.Description)}</Descripcion>");
            sb.Append($"<Cantidad>{F2(item.Quantity)}</Cantidad>");
            sb.Append($"<Precio>{F2(item.Price)}</Precio>");
            sb.Append($"<Medida>{item.MeasureUnit}</Medida>");
            sb.Append($"<ID_UNIDAD>{item.MeasureUnit}</ID_UNIDAD>");
            sb.Append($"<PorcDesc>{F2(item.DiscountPercentage)}</PorcDesc>");
            sb.Append($"<ImpBruto>{F2(item.GrossAmount)}</ImpBruto>");
            sb.Append($"<ImpDescuento>{F2(item.DiscountAmount)}</ImpDescuento>");
            sb.Append($"<ImpExento>{F2(item.ExemptAmount)}</ImpExento>");
            sb.Append($"<ImpOtros>{F2(item.OtherTaxes)}</ImpOtros>");
            sb.Append($"<ImpNeto>{F2(item.NetAmount)}</ImpNeto>");
            sb.Append($"<ImpIsr>{F2(item.IsrAmount)}</ImpIsr>");
            sb.Append($"<ImpIva>{F2(item.IvaAmount)}</ImpIva>");
            sb.Append($"<ImpTotal>{F2(item.TotalAmount)}</ImpTotal>");
            sb.Append($"<TipoVentaDet>{item.SaleType.ToCode()}</TipoVentaDet>");
            sb.Append("</Productos>");
            lineIndex++;
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

    // ==================== RESPONSE PARSING ====================

    private static ErrorOr<(string Serie, string Preimpreso, string NumeroAutorizacion)>
        ParseGeneraDocumentoResponse(string responseContent)
    {
        try
        {
            // Extract the result text from SOAP response
            var resultText = ExtractSoapResultText(responseContent);

            // Decode HTML entities (Ainnova returns HTML-encoded XML inside result)
            var decodedXml = HttpUtility.HtmlDecode(resultText);

            // Try to parse as XML
            var doc = XDocument.Parse($"<root>{decodedXml}</root>");

            var serie = doc.Descendants("Serie").FirstOrDefault()?.Value;
            var preimpreso = doc.Descendants("Preimpreso").FirstOrDefault()?.Value;
            var numeroAutorizacion = doc.Descendants("NumeroAutorizacion").FirstOrDefault()?.Value;

            if (!string.IsNullOrEmpty(serie) && !string.IsNullOrEmpty(preimpreso))
            {
                return (serie, preimpreso.Trim(), numeroAutorizacion ?? "");
            }

            // Also try "Resultado" wrapper (some responses wrap it)
            var resultado = doc.Descendants("Resultado").FirstOrDefault();
            if (resultado is not null)
            {
                serie = resultado.Element("Serie")?.Value;
                preimpreso = resultado.Element("Preimpreso")?.Value;
                numeroAutorizacion = resultado.Element("NumeroAutorizacion")?.Value;

                if (!string.IsNullOrEmpty(serie) && !string.IsNullOrEmpty(preimpreso))
                {
                    return (serie, preimpreso.Trim(), numeroAutorizacion ?? "");
                }
            }

            // Extract clean error text from <Resultado> if present
            var errorText = resultado?.Value ?? decodedXml;
            // Remove XML tags if any remain
            errorText = Regex.Replace(errorText, @"<[^>]+>", "").Trim();

            return Error.Failure(
                code: "FiscalDocument.GenerationFailed",
                description: string.IsNullOrEmpty(errorText) ? resultText : errorText);
        }
        catch (Exception ex)
        {
            return Error.Failure(
                code: "FiscalDocument.ParseError",
                description: $"No se pudo interpretar la respuesta: {ex.Message}");
        }
    }

    private static ErrorOr<bool> ParseAnulaDocumentoResponse(string responseContent)
    {
        try
        {
            var resultText = ExtractSoapResultText(responseContent);
            var decodedXml = HttpUtility.HtmlDecode(resultText);

            var doc = XDocument.Parse($"<root>{decodedXml}</root>");

            var estado = doc.Descendants("ESTADO").FirstOrDefault()?.Value
                      ?? doc.Descendants("Estado").FirstOrDefault()?.Value;

            if (estado == "ANULADO")
                return true;

            return Error.Failure(
                code: "FiscalDocument.VoidFailed",
                description: $"No se pudo anular el documento: {resultText}");
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
            var resultText = ExtractSoapResultText(responseContent);
            var decodedXml = HttpUtility.HtmlDecode(resultText);

            var doc = XDocument.Parse($"<root>{decodedXml}</root>");

            var numeros = doc.Descendants("NumeroAcceso")
                .Select(n => long.Parse(n.Value))
                .ToList();

            if (numeros.Count == 0)
            {
                return Error.Failure(
                    code: "FiscalDocument.NoContingencyNumbers",
                    description: $"No se recibieron números de contingencia: {resultText}");
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

    // ==================== HELPERS ====================

    /// <summary>
    /// Extracts the text content from SOAP result element.
    /// Handles both generaDocumentoResponse/result and anulaDocumentoResponse/result.
    /// </summary>
    private static string ExtractSoapResultText(string soapResponse)
    {
        try
        {
            // Remove non-printable characters (same as Odoo: re.sub(r'[^\x20-\x7E]', '', data))
            var cleaned = Regex.Replace(soapResponse, @"[^\x20-\x7E\r\n\t]", "");

            var doc = XDocument.Parse(cleaned);
            XNamespace ns = "http://dbguatefac/Guatefac.wsdl";

            // Try generaDocumentoResponse
            var result = doc.Descendants(ns + "generaDocumentoResponse")
                .Elements("result").FirstOrDefault();

            // Try anulaDocumentoResponse
            result ??= doc.Descendants(ns + "anulaDocumentoResponse")
                .Elements("result").FirstOrDefault();

            // Try generaNumerosContingenciaResponse
            result ??= doc.Descendants(ns + "generaNumerosContingenciaResponse")
                .Elements("result").FirstOrDefault();

            // Fallback: try any "result" element
            result ??= doc.Descendants("result").FirstOrDefault();

            return result?.Value ?? soapResponse;
        }
        catch
        {
            // If XML parsing fails, try regex extraction
            var match = Regex.Match(soapResponse, @"<result[^>]*>(.*?)</result>", RegexOptions.Singleline);
            return match.Success ? match.Groups[1].Value : soapResponse;
        }
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

    // ==================== TAX CALCULATION ====================

    private static List<CalculatedItem> CalculateItems(
        List<GenerateDocumentItemRequest> items,
        decimal taxPercentage)
    {
        return items.Select(item =>
        {
            // Calculate each item with 2 decimals (same as Odoo: imp_bruto = qty * price)
            var grossAmount = Math.Round(item.Quantity * item.Price, 2);
            var discountAmount = Math.Round(grossAmount * (item.DiscountPercentage / 100m), 2);
            var subtotal = Math.Round(grossAmount - discountAmount, 2);

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
                // IVA included in price (same as Odoo: imp_neto = imp_bruto / 1.12)
                netAmount = Math.Round(subtotal / (1 + taxPercentage), 2);
                ivaAmount = Math.Round(subtotal - netAmount, 2);
            }

            var totalAmount = Math.Round(subtotal, 2);

            return new CalculatedItem(
                ProductCode: item.ProductCode,
                Description: item.Description,
                MeasureUnit: item.MeasureUnit,
                Quantity: Math.Round(item.Quantity, 2),
                Price: Math.Round(item.Price, 2),
                DiscountPercentage: Math.Round(item.DiscountPercentage, 2),
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
