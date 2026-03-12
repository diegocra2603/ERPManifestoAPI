using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;
using Domain.Contracts.Infrastructure.Persistence;
using Domain.Contracts.Infrastructure.Services.FiscalDataValidator;
using Domain.Primitives;
using Domain.Entities.FiscalData;
using Domain.ValueObjects;
using ErrorOr;
using Microsoft.Extensions.Options;

namespace Services.FiscalDataValidator;

public class InnovaFiscalDataValidatorService : IFiscalDataValidatorService
{
    private readonly FiscalDataValidatorConfiguration _configuration;
    private readonly IAsyncRepository<FiscalDataEntry> _fiscalDataRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpClientFactory _httpClientFactory;

    public InnovaFiscalDataValidatorService(
        IOptions<FiscalDataValidatorConfiguration> configuration,
        IAsyncRepository<FiscalDataEntry> fiscalDataRepository,
        IUnitOfWork unitOfWork,
        IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration.Value;
        _fiscalDataRepository = fiscalDataRepository;
        _unitOfWork = unitOfWork;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ErrorOr<FiscalDataEntry>> ValidateFiscalDataAsync(string taxId)
    {
        var existingEntry = await _fiscalDataRepository.FirstOrDefaultAsync(
            f => f.FiscalCode == taxId && f.AuditField.IsActive);

        if (existingEntry is not null)
            return existingEntry;

        var result = await CallInnovaServiceAsync(taxId);

        if (result.IsError)
            return result.Errors;

        var (fiscalCode, fiscalName) = result.Value;

        var fiscalDataEntry = new FiscalDataEntry(
            new FiscalDataEntryId(Guid.NewGuid()),
            fiscalCode,
            fiscalName,
            AuditField.Create());

        _fiscalDataRepository.Add(fiscalDataEntry);
        await _unitOfWork.SaveChangesAsync();

        return fiscalDataEntry;
    }

    private async Task<ErrorOr<(string FiscalCode, string FiscalName)>> CallInnovaServiceAsync(string taxId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();

            var credentials = Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{_configuration.UserHeader}:{_configuration.PasswordHeader}"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", credentials);

            var parametrosHeader = $"<Servicio>ValidaNIT</Servicio>" +
                                   $"<Usuario>{_configuration.User}</Usuario>" +
                                   $"<Clave>{_configuration.Password}</Clave>" +
                                   $"<Nit>{taxId}</Nit>";

            var request = new HttpRequestMessage(HttpMethod.Post, _configuration.Url);
            request.Headers.TryAddWithoutValidation("PARAMETROS", parametrosHeader);

            var xmlBody = $"<pNit>{taxId}</pNit>";
            request.Content = new StringContent(xmlBody, Encoding.UTF8, "text/xml");

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return ParseResponse(responseContent, taxId);
        }
        catch (Exception ex)
        {
            return Error.Failure(
                code: "FiscalDataValidator.ServiceError",
                description: $"Error al consultar el servicio de validacion fiscal: {ex.Message}");
        }
    }

    private static ErrorOr<(string FiscalCode, string FiscalName)> ParseResponse(string xmlResponse, string taxId)
    {
        try
        {
            var doc = XDocument.Parse($"<root>{xmlResponse}</root>");
            var respuesta = doc.Descendants("RESPUESTA").FirstOrDefault();

            if (respuesta is null)
            {
                return Error.Failure(
                    code: "FiscalDataValidator.InvalidResponse",
                    description: "La respuesta del servicio no tiene el formato esperado.");
            }

            var nitElement = respuesta.Element("NIT");
            var nombreElement = respuesta.Element("NOMBRE");

            if (nitElement is not null && nombreElement is not null)
            {
                return (nitElement.Value, nombreElement.Value);
            }

            var errorMessage = respuesta.Value;
            return Error.NotFound(
                code: "FiscalDataValidator.InvalidTaxId",
                description: $"NIT invalido: {errorMessage}");
        }
        catch
        {
            if (xmlResponse.Contains("NIT INVALIDO", StringComparison.OrdinalIgnoreCase))
            {
                return Error.NotFound(
                    code: "FiscalDataValidator.InvalidTaxId",
                    description: "NIT INVALIDO");
            }

            return Error.Failure(
                code: "FiscalDataValidator.ParseError",
                description: "No se pudo interpretar la respuesta del servicio de validacion fiscal.");
        }
    }
}
