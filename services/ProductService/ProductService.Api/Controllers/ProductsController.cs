using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ProductService.Api.Controllers;

[ApiController]
[Route("api/v1/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductConfigurationRepository _repository;

    public ProductsController(
        IProductConfigurationRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{productCode}/attributes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductAttributes(
        string productCode)
    {
        var product =
            await _repository.GetProductByCodeAsync(productCode);

        if (product is null)
        {
            return NotFound(new
            {
                status = 404,
                message = "Proizvod nije pronađen.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        var attributes =
            await _repository.GetAttributesByProductCodeAsync(
                productCode);

        var items = attributes
            .Select(MapProductAttribute)
            .ToList();

        return Ok(new
        {
            items,
            totalCount = items.Count
        });
    }

    private static object MapProductAttribute(
        ProductAttribute attribute)
    {
        return new
        {
            attributeCode = attribute.AttributeCode,
            productCode = attribute.ProductCode,
            name = attribute.Name,
            description = attribute.Description,
            isRequired = attribute.IsRequired,
            displayOrder = attribute.DisplayOrder,
            status = attribute.Status,
        };
    }
}