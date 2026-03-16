using Application.Common.Constants;
using Application.Features.TaxConfigurations.Create;
using Application.Features.TaxConfigurations.Delete;
using Application.Features.TaxConfigurations.GetAll;
using Application.Features.TaxConfigurations.Update;
using Domain.Primitives.Mediator;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class TaxConfigurationController : ApiController
{
    private readonly ISender _mediator;

    public TaxConfigurationController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [HasPermission(PermissionCodes.AccountingCreate)]
    public async Task<IActionResult> CreateTaxConfiguration([FromBody] CreateTaxConfigurationCommand command)
    {
        var result = await _mediator.Send(command);

        return result.Match(
            tax => Ok(tax),
            errors => Problem(errors)
        );
    }

    [HttpGet]
    [HasPermission(PermissionCodes.AccountingRead)]
    public async Task<IActionResult> GetAllTaxConfigurations()
    {
        var result = await _mediator.Send(new GetAllTaxConfigurationsQuery());

        return result.Match(
            taxes => Ok(taxes),
            errors => Problem(errors)
        );
    }

    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.AccountingUpdate)]
    public async Task<IActionResult> UpdateTaxConfiguration(Guid id, [FromBody] UpdateTaxConfigurationCommand command)
    {
        if (id != command.Id)
            return BadRequest("El id de la ruta no coincide con el del cuerpo.");

        var result = await _mediator.Send(command);

        return result.Match(
            tax => Ok(tax),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.AccountingDelete)]
    public async Task<IActionResult> DeleteTaxConfiguration(Guid id)
    {
        var result = await _mediator.Send(new DeleteTaxConfigurationCommand(id));

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }
}
