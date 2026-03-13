using Application.Common.Constants;
using Application.Features.Briefs.Create;
using Application.Features.Briefs.Delete;
using Application.Features.Briefs.Get;
using Application.Features.Briefs.GetByProduct;
using Application.Features.Briefs.Update;
using Domain.Primitives.Mediator;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class BriefController : ApiController
{
    private readonly ISender _mediator;

    public BriefController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [HasPermission(PermissionCodes.BriefCreate)]
    public async Task<IActionResult> CreateBrief([FromBody] CreateBriefCommand command)
    {
        var result = await _mediator.Send(command);

        return result.Match(
            brief => Ok(brief),
            errors => Problem(errors)
        );
    }

    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.BriefRead)]
    public async Task<IActionResult> GetBriefById([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetBriefByIdQuery(id));

        return result.Match(
            brief => Ok(brief),
            errors => Problem(errors)
        );
    }

    [HttpGet("by-product/{productId:guid}")]
    [HasPermission(PermissionCodes.BriefRead)]
    public async Task<IActionResult> GetBriefsByProduct([FromRoute] Guid productId)
    {
        var result = await _mediator.Send(new GetBriefsByProductQuery(productId));

        return result.Match(
            briefs => Ok(briefs),
            errors => Problem(errors)
        );
    }

    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.BriefUpdate)]
    public async Task<IActionResult> UpdateBrief(Guid id, [FromBody] UpdateBriefCommand command)
    {
        if (id != command.Id)
            return BadRequest("El id de la ruta no coincide con el del cuerpo.");

        var result = await _mediator.Send(command);

        return result.Match(
            brief => Ok(brief),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.BriefDelete)]
    public async Task<IActionResult> DeleteBrief(Guid id)
    {
        var result = await _mediator.Send(new DeleteBriefCommand(id));

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }
}
