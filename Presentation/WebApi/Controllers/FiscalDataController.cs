using Domain.Contracts.Infrastructure.Services.FiscalDataValidator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class FiscalDataController : ApiController
{
    private readonly IFiscalDataValidatorService _fiscalDataValidatorService;

    public FiscalDataController(IFiscalDataValidatorService fiscalDataValidatorService)
    {
        _fiscalDataValidatorService = fiscalDataValidatorService;
    }

    [HttpGet("validate/{taxId}")]
    [Authorize]
    public async Task<IActionResult> ValidateFiscalData([FromRoute] string taxId)
    {
        var result = await _fiscalDataValidatorService.ValidateFiscalDataAsync(taxId);

        return result.Match(
            fiscalData => Ok(new
            {
                fiscalData.FiscalCode,
                fiscalData.FiscalName
            }),
            errors => Problem(errors)
        );
    }
}
