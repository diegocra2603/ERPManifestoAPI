using Application.Common.Constants;
using Application.Features.JobPositions.Create;
using Application.Features.JobPositions.Delete;
using Application.Features.JobPositions.Get;
using Application.Features.JobPositions.GetAll;
using Application.Features.JobPositions.Update;
using Domain.Primitives.Mediator;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class JobPositionController : ApiController
{
    private readonly ISender _mediator;

    public JobPositionController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [HasPermission(PermissionCodes.JobPositionCreate)]
    public async Task<IActionResult> CreateJobPosition([FromBody] CreateJobPositionCommand command)
    {
        var result = await _mediator.Send(command);

        return result.Match(
            jobPosition => Ok(jobPosition),
            errors => Problem(errors)
        );
    }

    [HttpGet]
    [HasPermission(PermissionCodes.JobPositionRead)]
    public async Task<IActionResult> GetAllJobPositions()
    {
        var result = await _mediator.Send(new GetAllJobPositionsQuery());

        return result.Match(
            jobPositions => Ok(jobPositions),
            errors => Problem(errors)
        );
    }

    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.JobPositionRead)]
    public async Task<IActionResult> GetJobPositionById([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetJobPositionByIdQuery(id));

        return result.Match(
            jobPosition => Ok(jobPosition),
            errors => Problem(errors)
        );
    }

    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.JobPositionUpdate)]
    public async Task<IActionResult> UpdateJobPosition(Guid id, [FromBody] UpdateJobPositionCommand command)
    {
        if (id != command.Id)
            return BadRequest("El id de la ruta no coincide con el del cuerpo.");

        var result = await _mediator.Send(command);

        return result.Match(
            jobPosition => Ok(jobPosition),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.JobPositionDelete)]
    public async Task<IActionResult> DeleteJobPosition(Guid id)
    {
        var result = await _mediator.Send(new DeleteJobPositionCommand(id));

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }
}
