using Application.Common.Constants;
using Application.Features.ExchangeRates.Create;
using Application.Features.ExchangeRates.Delete;
using Application.Features.ExchangeRates.GetAll;
using Application.Features.ExchangeRates.Update;
using Domain.Primitives.Mediator;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class ExchangeRateController : ApiController
{
    private readonly ISender _mediator;

    public ExchangeRateController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [HasPermission(PermissionCodes.AccountingCreate)]
    public async Task<IActionResult> CreateExchangeRate([FromBody] CreateExchangeRateCommand command)
    {
        var result = await _mediator.Send(command);

        return result.Match(
            rate => Ok(rate),
            errors => Problem(errors)
        );
    }

    [HttpGet]
    [HasPermission(PermissionCodes.AccountingRead)]
    public async Task<IActionResult> GetAllExchangeRates()
    {
        var result = await _mediator.Send(new GetAllExchangeRatesQuery());

        return result.Match(
            rates => Ok(rates),
            errors => Problem(errors)
        );
    }

    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.AccountingUpdate)]
    public async Task<IActionResult> UpdateExchangeRate(Guid id, [FromBody] UpdateExchangeRateCommand command)
    {
        if (id != command.Id)
            return BadRequest("El id de la ruta no coincide con el del cuerpo.");

        var result = await _mediator.Send(command);

        return result.Match(
            rate => Ok(rate),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.AccountingDelete)]
    public async Task<IActionResult> DeleteExchangeRate(Guid id)
    {
        var result = await _mediator.Send(new DeleteExchangeRateCommand(id));

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }
}
