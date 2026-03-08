using Application.Common.Constants;
using Application.Features.Roles.Get;
using Application.Features.Roles.GetAll;
using Domain.Primitives.Mediator;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class RoleController : ApiController
{
    private readonly ISender _mediator;

    public RoleController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [HasPermission(PermissionCodes.RoleRead)]
    public async Task<IActionResult> GetAllRoles()
    {
        var result = await _mediator.Send(new GetAllRolesQuery());

        return result.Match(
            roles => Ok(roles),
            errors => Problem(errors)
        );
    }

    [HttpGet("{id}")]
    [HasPermission(PermissionCodes.RoleRead)]
    public async Task<IActionResult> GetRoleById([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetRoleByIdQuery(id));
        
        return result.Match(
            role => Ok(role),
            errors => Problem(errors)
        );  
    }
}