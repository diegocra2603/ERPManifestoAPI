using Application.Common.Constants;
using Application.Features.Clients.Create;
using Application.Features.Clients.Delete;
using Application.Features.Clients.Get;
using Application.Features.Clients.GetAll;
using Application.Features.Clients.Update;
using Domain.Primitives.Mediator;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class ClientController : ApiController
{
    private readonly ISender _mediator;

    public ClientController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [HasPermission(PermissionCodes.AccountingCreate)]
    public async Task<IActionResult> CreateClient([FromBody] CreateClientCommand command)
    {
        var result = await _mediator.Send(command);

        return result.Match(
            client => Ok(client),
            errors => Problem(errors)
        );
    }

    [HttpGet]
    [HasPermission(PermissionCodes.AccountingRead)]
    public async Task<IActionResult> GetAllClients()
    {
        var result = await _mediator.Send(new GetAllClientsQuery());

        return result.Match(
            clients => Ok(clients),
            errors => Problem(errors)
        );
    }

    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.AccountingRead)]
    public async Task<IActionResult> GetClientById(Guid id)
    {
        var result = await _mediator.Send(new GetClientByIdQuery(id));

        return result.Match(
            client => Ok(client),
            errors => Problem(errors)
        );
    }

    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.AccountingUpdate)]
    public async Task<IActionResult> UpdateClient(Guid id, [FromBody] UpdateClientCommand command)
    {
        if (id != command.Id)
            return BadRequest("El id de la ruta no coincide con el del cuerpo.");

        var result = await _mediator.Send(command);

        return result.Match(
            client => Ok(client),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.AccountingDelete)]
    public async Task<IActionResult> DeleteClient(Guid id)
    {
        var result = await _mediator.Send(new DeleteClientCommand(id));

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }
}
