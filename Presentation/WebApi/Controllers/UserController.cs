using Application.Common.Constants;
using Application.Features.Users.Create;
using Application.Features.Users.CreateWithPassword;
using Application.Features.Users.Delete;
using Application.Features.Users.Get;
using Application.Features.Users.GetAll;
using Application.Features.Users.ResetPassword;
using Application.Features.Users.Update;
using Domain.Primitives.Mediator;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;
using WebApi.Controllers;

namespace Presentation.WebApi.Controllers;

[Route("api/[controller]")]
public class UserController : ApiController
{
    private readonly ISender _mediator;

    public UserController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [HasPermission(PermissionCodes.UserCreate)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        var result = await _mediator.Send(command);

        return result.Match(
            user => Ok(user),
            errors => Problem(errors)
        );
    }

    [HttpPost("with-password")]
    [HasPermission(PermissionCodes.UserCreate)]
    public async Task<IActionResult> CreateUserWithPassword([FromBody] CreateUserWithPasswordCommand command)
    {
        var result = await _mediator.Send(command);

        return result.Match(
            user => Ok(user),
            errors => Problem(errors)
        );
    }

    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.UserUpdate)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.Id)
            return BadRequest("El id de la ruta no coincide con el del cuerpo.");

        var result = await _mediator.Send(command);

        return result.Match(
            user => Ok(user),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{userId:guid}")]
    [HasPermission(PermissionCodes.UserDelete)]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        var result = await _mediator.Send(new DeleteUserCommand(userId));

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }

    [HttpPost("reset-password")]
    [HasPermission(PermissionCodes.UserRead)]
    public async Task<IActionResult> ResetPassword()
    {
        var result = await _mediator.Send(new ResetPasswordCommand());

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }

    [HttpPost("reset-password-by-admin")]
    [HasPermission(PermissionCodes.UserUpdate)]
    public async Task<IActionResult> ResetPasswordByAdmin([FromBody] ResetPasswordCommand command)
    {
        var result = await _mediator.Send(command);

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }

    [HttpGet]
    [HasPermission(PermissionCodes.UserRead)]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _mediator.Send(new GetAllUsersQuery());

        return result.Match(
            users => Ok(users),
            errors => Problem(errors)
        );
    }

    [HttpGet("{id}")]
    [HasPermission(PermissionCodes.UserRead)]
    public async Task<IActionResult> GetUserById([FromRoute] string id)
    {
        var result = await _mediator.Send(new GetUserByParamQuery(id));

        return result.Match(
            user => Ok(user),
            errors => Problem(errors)
        );
    }
}