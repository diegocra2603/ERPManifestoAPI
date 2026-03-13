using Application.Common.Constants;
using Application.Features.SystemSettings.GetAll;
using Application.Features.SystemSettings.Update;
using Domain.Primitives.Mediator;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class SystemSettingController : ApiController
{
    private readonly ISender _mediator;

    public SystemSettingController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [HasPermission(PermissionCodes.SettingRead)]
    public async Task<IActionResult> GetAllSettings()
    {
        var result = await _mediator.Send(new GetAllSystemSettingsQuery());

        return result.Match(
            settings => Ok(settings),
            errors => Problem(errors)
        );
    }

    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.SettingUpdate)]
    public async Task<IActionResult> UpdateSetting(Guid id, [FromBody] UpdateSystemSettingCommand command)
    {
        if (id != command.Id)
            return BadRequest("El id de la ruta no coincide con el del cuerpo.");

        var result = await _mediator.Send(command);

        return result.Match(
            setting => Ok(setting),
            errors => Problem(errors)
        );
    }
}
