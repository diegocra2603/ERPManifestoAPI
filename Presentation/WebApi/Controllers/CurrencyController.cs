using Application.Common.Constants;
using Application.Features.Currencies.Create;
using Application.Features.Currencies.Delete;
using Application.Features.Currencies.GetAll;
using Application.Features.Currencies.Update;
using Domain.Primitives.Mediator;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class CurrencyController : ApiController
{
    private readonly ISender _mediator;

    public CurrencyController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [HasPermission(PermissionCodes.AccountingCreate)]
    public async Task<IActionResult> CreateCurrency([FromBody] CreateCurrencyCommand command)
    {
        var result = await _mediator.Send(command);

        return result.Match(
            currency => Ok(currency),
            errors => Problem(errors)
        );
    }

    [HttpGet]
    [HasPermission(PermissionCodes.AccountingRead)]
    public async Task<IActionResult> GetAllCurrencies()
    {
        var result = await _mediator.Send(new GetAllCurrenciesQuery());

        return result.Match(
            currencies => Ok(currencies),
            errors => Problem(errors)
        );
    }

    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.AccountingUpdate)]
    public async Task<IActionResult> UpdateCurrency(Guid id, [FromBody] UpdateCurrencyCommand command)
    {
        if (id != command.Id)
            return BadRequest("El id de la ruta no coincide con el del cuerpo.");

        var result = await _mediator.Send(command);

        return result.Match(
            currency => Ok(currency),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.AccountingDelete)]
    public async Task<IActionResult> DeleteCurrency(Guid id)
    {
        var result = await _mediator.Send(new DeleteCurrencyCommand(id));

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }
}
