using Application.Common.Constants;
using Application.Features.AccountCatalogs.Create;
using Application.Features.AccountCatalogs.Delete;
using Application.Features.AccountCatalogs.GetAll;
using Application.Features.AccountCatalogs.Update;
using Domain.Primitives.Mediator;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class AccountCatalogController : ApiController
{
    private readonly ISender _mediator;

    public AccountCatalogController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [HasPermission(PermissionCodes.AccountingCreate)]
    public async Task<IActionResult> CreateAccountCatalog([FromBody] CreateAccountCatalogCommand command)
    {
        var result = await _mediator.Send(command);

        return result.Match(
            account => Ok(account),
            errors => Problem(errors)
        );
    }

    [HttpGet]
    [HasPermission(PermissionCodes.AccountingRead)]
    public async Task<IActionResult> GetAllAccountCatalogs()
    {
        var result = await _mediator.Send(new GetAllAccountCatalogsQuery());

        return result.Match(
            accounts => Ok(accounts),
            errors => Problem(errors)
        );
    }

    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.AccountingUpdate)]
    public async Task<IActionResult> UpdateAccountCatalog(Guid id, [FromBody] UpdateAccountCatalogCommand command)
    {
        if (id != command.Id)
            return BadRequest("El id de la ruta no coincide con el del cuerpo.");

        var result = await _mediator.Send(command);

        return result.Match(
            account => Ok(account),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.AccountingDelete)]
    public async Task<IActionResult> DeleteAccountCatalog(Guid id)
    {
        var result = await _mediator.Send(new DeleteAccountCatalogCommand(id));

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }
}
