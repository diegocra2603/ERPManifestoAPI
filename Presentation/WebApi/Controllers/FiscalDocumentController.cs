using Domain.Contracts.Infrastructure.Services.FiscalDocument;
using Domain.Entities.FiscalDocuments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class FiscalDocumentController : ApiController
{
    private readonly IFiscalDocumentService _fiscalDocumentService;

    public FiscalDocumentController(IFiscalDocumentService fiscalDocumentService)
    {
        _fiscalDocumentService = fiscalDocumentService;
    }

    [HttpPost("generate")]
    [Authorize]
    public async Task<IActionResult> GenerateDocument([FromBody] GenerateDocumentRequest request)
    {
        var result = await _fiscalDocumentService.GenerateDocumentAsync(request);

        return result.Match(
            document => Ok(new
            {
                document.Id,
                DocumentType = document.DocumentType.ToString(),
                Status = document.Status.ToString(),
                document.NitReceptor,
                document.NombreReceptor,
                document.Referencia,
                document.Serie,
                document.Preimpreso,
                document.NumeroAutorizacion,
                document.NumeroAcceso,
                document.Total,
                document.Fecha
            }),
            errors => Problem(errors));
    }

    [HttpPost("void")]
    [Authorize]
    public async Task<IActionResult> VoidDocument([FromBody] VoidDocumentRequest request)
    {
        var result = await _fiscalDocumentService.VoidDocumentAsync(request);

        return result.Match(
            document => Ok(new
            {
                document.Id,
                Status = document.Status.ToString(),
                document.Serie,
                document.Preimpreso
            }),
            errors => Problem(errors));
    }

    [HttpPost("contingency/numbers")]
    [Authorize]
    public async Task<IActionResult> GenerateContingencyNumbers([FromBody] ContingencyNumbersRequest request)
    {
        var result = await _fiscalDocumentService.GenerateContingencyNumbersAsync(request.Cantidad, request.Lote);

        return result.Match(
            numbers => Ok(new { NumerosAcceso = numbers, Cantidad = numbers.Count }),
            errors => Problem(errors));
    }

    [HttpPost("contingency/upload")]
    [Authorize]
    public async Task<IActionResult> UploadContingencyDocuments()
    {
        var result = await _fiscalDocumentService.UploadContingencyDocumentsAsync();

        return result.Match(
            documents => Ok(new
            {
                DocumentosCargados = documents.Count,
                Documentos = documents.Select(d => new
                {
                    d.Id,
                    d.Referencia,
                    d.Serie,
                    d.Preimpreso,
                    d.NumeroAutorizacion,
                    Status = d.Status.ToString()
                })
            }),
            errors => Problem(errors));
    }
}

public record ContingencyNumbersRequest(int Cantidad, string Lote);
