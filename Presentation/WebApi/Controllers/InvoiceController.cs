using Application.Common.Constants;
using Application.Features.Invoices.CreatePayable;
using Application.Features.Invoices.CreateReceivable;
using Application.Features.Invoices.Delete;
using Application.Features.Invoices.Emit;
using Application.Features.Invoices.Get;
using Application.Features.Invoices.GetByType;
using Application.Features.Invoices.Void;
using Domain.Primitives.Mediator;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class InvoiceController : ApiController
{
    private readonly ISender _mediator;

    public InvoiceController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("receivable")]
    [HasPermission(PermissionCodes.AccountingCreate)]
    public async Task<IActionResult> CreateReceivableInvoice([FromBody] CreateReceivableInvoiceCommand command)
    {
        var result = await _mediator.Send(command);

        return result.Match(
            invoice => Ok(invoice),
            errors => Problem(errors)
        );
    }

    [HttpPost("payable")]
    [HasPermission(PermissionCodes.AccountingCreate)]
    public async Task<IActionResult> CreatePayableInvoice([FromBody] CreatePayableInvoiceCommand command)
    {
        var result = await _mediator.Send(command);

        return result.Match(
            invoice => Ok(invoice),
            errors => Problem(errors)
        );
    }

    [HttpGet("receivable")]
    [HasPermission(PermissionCodes.AccountingRead)]
    public async Task<IActionResult> GetReceivableInvoices()
    {
        var result = await _mediator.Send(new GetInvoicesByTypeQuery(1));

        return result.Match(
            invoices => Ok(invoices),
            errors => Problem(errors)
        );
    }

    [HttpGet("payable")]
    [HasPermission(PermissionCodes.AccountingRead)]
    public async Task<IActionResult> GetPayableInvoices()
    {
        var result = await _mediator.Send(new GetInvoicesByTypeQuery(2));

        return result.Match(
            invoices => Ok(invoices),
            errors => Problem(errors)
        );
    }

    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.AccountingRead)]
    public async Task<IActionResult> GetInvoiceById(Guid id)
    {
        var result = await _mediator.Send(new GetInvoiceByIdQuery(id));

        return result.Match(
            invoice => Ok(invoice),
            errors => Problem(errors)
        );
    }

    [HttpPost("{id:guid}/emit")]
    [HasPermission(PermissionCodes.AccountingUpdate)]
    public async Task<IActionResult> EmitInvoice(Guid id)
    {
        var result = await _mediator.Send(new EmitInvoiceCommand(id));

        return result.Match(
            _ => Ok(_),
            errors => Problem(errors)
        );
    }

    [HttpPost("{id:guid}/void")]
    [HasPermission(PermissionCodes.AccountingUpdate)]
    public async Task<IActionResult> VoidInvoice(Guid id)
    {
        var result = await _mediator.Send(new VoidInvoiceCommand(id));

        return result.Match(
            _ => Ok(_),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.AccountingDelete)]
    public async Task<IActionResult> DeleteInvoice(Guid id)
    {
        var result = await _mediator.Send(new DeleteInvoiceCommand(id));

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }
}
