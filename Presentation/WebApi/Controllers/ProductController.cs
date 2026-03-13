using Application.Common.Constants;
using Application.Features.Products.AddJobPosition;
using Application.Features.Products.Create;
using Application.Features.Products.Delete;
using Application.Features.Products.Get;
using Application.Features.Products.GetAll;
using Application.Features.Products.RemoveJobPosition;
using Application.Features.Products.Update;
using Domain.Primitives.Mediator;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class ProductController : ApiController
{
    private readonly ISender _mediator;

    public ProductController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [HasPermission(PermissionCodes.ProductCreate)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        var result = await _mediator.Send(command);

        return result.Match(
            product => Ok(product),
            errors => Problem(errors)
        );
    }

    [HttpGet]
    [HasPermission(PermissionCodes.ProductRead)]
    public async Task<IActionResult> GetAllProducts()
    {
        var result = await _mediator.Send(new GetAllProductsQuery());

        return result.Match(
            products => Ok(products),
            errors => Problem(errors)
        );
    }

    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.ProductRead)]
    public async Task<IActionResult> GetProductById([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id));

        return result.Match(
            product => Ok(product),
            errors => Problem(errors)
        );
    }

    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.ProductUpdate)]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.Id)
            return BadRequest("El id de la ruta no coincide con el del cuerpo.");

        var result = await _mediator.Send(command);

        return result.Match(
            product => Ok(product),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.ProductDelete)]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var result = await _mediator.Send(new DeleteProductCommand(id));

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }

    [HttpPost("{id:guid}/job-positions")]
    [HasPermission(PermissionCodes.ProductUpdate)]
    public async Task<IActionResult> AddJobPositionToProduct(Guid id, [FromBody] AddJobPositionToProductCommand command)
    {
        if (id != command.ProductId)
            return BadRequest("El id de la ruta no coincide con el del cuerpo.");

        var result = await _mediator.Send(command);

        return result.Match(
            product => Ok(product),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{id:guid}/job-positions/{productJobPositionId:guid}")]
    [HasPermission(PermissionCodes.ProductUpdate)]
    public async Task<IActionResult> RemoveJobPositionFromProduct(Guid id, Guid productJobPositionId)
    {
        var result = await _mediator.Send(new RemoveJobPositionFromProductCommand(id, productJobPositionId));

        return result.Match(
            product => Ok(product),
            errors => Problem(errors)
        );
    }
}
