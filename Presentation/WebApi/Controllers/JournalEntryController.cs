using Application.Common.Constants;
using Application.Features.JournalEntries.Approve;
using Application.Features.JournalEntries.Create;
using Application.Features.JournalEntries.Delete;
using Application.Features.JournalEntries.Get;
using Application.Features.JournalEntries.GetAll;
using Application.Features.JournalEntries.Void;
using Domain.Primitives.Mediator;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class JournalEntryController : ApiController
{
    private readonly ISender _mediator;

    public JournalEntryController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [HasPermission(PermissionCodes.AccountingCreate)]
    public async Task<IActionResult> CreateJournalEntry([FromBody] CreateJournalEntryCommand command)
    {
        var result = await _mediator.Send(command);

        return result.Match(
            entry => Ok(entry),
            errors => Problem(errors)
        );
    }

    [HttpGet]
    [HasPermission(PermissionCodes.AccountingRead)]
    public async Task<IActionResult> GetAllJournalEntries()
    {
        var result = await _mediator.Send(new GetAllJournalEntriesQuery());

        return result.Match(
            entries => Ok(entries),
            errors => Problem(errors)
        );
    }

    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.AccountingRead)]
    public async Task<IActionResult> GetJournalEntryById(Guid id)
    {
        var result = await _mediator.Send(new GetJournalEntryByIdQuery(id));

        return result.Match(
            entry => Ok(entry),
            errors => Problem(errors)
        );
    }

    [HttpPost("{id:guid}/approve")]
    [HasPermission(PermissionCodes.AccountingUpdate)]
    public async Task<IActionResult> ApproveJournalEntry(Guid id)
    {
        var result = await _mediator.Send(new ApproveJournalEntryCommand(id));

        return result.Match(
            entry => Ok(entry),
            errors => Problem(errors)
        );
    }

    [HttpPost("{id:guid}/void")]
    [HasPermission(PermissionCodes.AccountingUpdate)]
    public async Task<IActionResult> VoidJournalEntry(Guid id)
    {
        var result = await _mediator.Send(new VoidJournalEntryCommand(id));

        return result.Match(
            entry => Ok(entry),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.AccountingDelete)]
    public async Task<IActionResult> DeleteJournalEntry(Guid id)
    {
        var result = await _mediator.Send(new DeleteJournalEntryCommand(id));

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }
}
