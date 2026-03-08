using Application.Features.Auth.ChangePassword;
using Application.Features.Auth.Login;
using Application.Features.Auth.LoginWithCode;
using Application.Features.Auth.LoginWithDevice;
using Application.Features.Auth.LoginWithToken;
using Domain.Primitives.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class AuthController : ApiController
{
    private readonly ISender _mediator;

    public AuthController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        var result = await _mediator.Send(command);

        return result.Match(
            authResponse => Ok(authResponse),
            errors => Problem(errors)
        );
    }

    [HttpPost("login-with-code")]
    public async Task<IActionResult> LoginWithCode(LoginWithCodeCommand command)
    {
        var result = await _mediator.Send(command);

        return result.Match(
            authResponse => Ok(authResponse),
            errors => Problem(errors)
        );
    }

    [HttpPost("login-with-device")]
    public async Task<IActionResult> LoginWithDevice(LoginWithDeviceCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Match(
            authResponse => Ok(authResponse),
            errors => Problem(errors)
        );
    }

    [Authorize]
    [HttpGet("login-with-token")]
    public async Task<IActionResult> LoginWithToken()
    {
        var result = await _mediator.Send(new LoginWithTokenCommand());
        return result.Match(
            authResponse => Ok(authResponse),
            errors => Problem(errors)
        );
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Match(
            _ => Ok(),
            errors => Problem(errors)
        );
    }
}
