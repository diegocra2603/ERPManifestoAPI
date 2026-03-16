using Application.Common.Constants;
using Application.Features.AccountingPeriods.Close;
using Application.Features.AccountingPeriods.Create;
using Application.Features.AccountingPeriods.Delete;
using Application.Features.AccountingPeriods.GetAll;
using Application.Features.AccountingPeriods.Update;
using Domain.Primitives.Mediator;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class AccountingPeriodController : ApiController
{
    private readonly ISender _mediator;

    public AccountingPeriodController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [HasPermission(PermissionCodes.AccountingCreate)]
    public async Task<IActionResult> CreateAccountingPeriod([FromBody] CreateAccountingPeriodCommand command)
    {
        var result = await _mediator.Send(command);

        return result.Match(
            period => Ok(period),
            errors => Problem(errors)
        );
    }

    [HttpGet]
    [HasPermission(PermissionCodes.AccountingRead)]
    public async Task<IActionResult> GetAllAccountingPeriods()
    {
        var result = await _mediator.Send(new GetAllAccountingPeriodsQuery());

        return result.Match(
            periods => Ok(periods),
            errors => Problem(errors)
        );
    }

    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.AccountingUpdate)]
    public async Task<IActionResult> UpdateAccountingPeriod(Guid id, [FromBody] UpdateAccountingPeriodCommand command)
    {
        if (id != command.Id)
            return BadRequest("El id de la ruta no coincide con el del cuerpo.");

        var result = await _mediator.Send(command);

        return result.Match(
            period => Ok(period),
            errors => Problem(errors)
        );
    }

    [HttpPost("{id:guid}/close")]
    [HasPermission(PermissionCodes.AccountingClose)]
    public async Task<IActionResult> CloseAccountingPeriod(Guid id)
    {
        var result = await _mediator.Send(new CloseAccountingPeriodCommand(id));

        return result.Match(
            period => Ok(period),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.AccountingDelete)]
    public async Task<IActionResult> DeleteAccountingPeriod(Guid id)
    {
        var result = await _mediator.Send(new DeleteAccountingPeriodCommand(id));

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }
}
