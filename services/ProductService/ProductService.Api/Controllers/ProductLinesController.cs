using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ProductService.Api.Controllers;

[ApiController]
[Route("api/v1/product-lines")]
public class ProductLinesController : ControllerBase
{
    private readonly IProductConfigurationRepository _repository;

    public ProductLinesController(
        IProductConfigurationRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProductLines(
        [FromQuery] string? merchantCountry)
    {
        if (!string.IsNullOrWhiteSpace(merchantCountry)
            && merchantCountry.Trim().Length != 2)
        {
            return BadRequest(new
            {
                status = 400,
                message = "MerchantCountry mora biti ISO kod od 2 znaka.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        var productLines = await _repository.GetProductLinesAsync();

        var items = productLines
            .Select(MapProductLine)
            .ToList();

        return Ok(new
        {
            items,
            totalCount = items.Count
        });
    }

    [HttpGet("{productLineCode}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductLineByCode(
        string productLineCode)
    {
        var productLine =
            await _repository.GetProductLineByCodeAsync(
                productLineCode);

        if (productLine is null)
        {
            return NotFound(new
            {
                status = 404,
                message = "Product line nije pronađen.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        return Ok(MapProductLine(productLine));
    }

    [HttpGet("{productLineCode}/products")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductsByProductLine(
        string productLineCode)
    {
        var productLine =
            await _repository.GetProductLineByCodeAsync(
                productLineCode);

        if (productLine is null)
        {
            return NotFound(new
            {
                status = 404,
                message = "Product line nije pronađen.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        var products =
            await _repository.GetProductsByProductLineAsync(
                productLineCode);

        var items = products
            .Select(MapProduct)
            .ToList();

        return Ok(new
        {
            items,
            totalCount = items.Count
        });
    }

    private static object MapProductLine(ProductLine productLine)
    {
        return new
        {
            productLineCode = productLine.ProductLineCode,
            name = productLine.Name,
            description = productLine.Description,
            requiresProductSelection =
                productLine.RequiresProductSelection,
            displayOrder = productLine.DisplayOrder,
            status = productLine.Status
        };
    }

    private static object MapProduct(Product product)
    {
        return new
        {
            productCode = product.ProductCode,
            productLineCode = product.ProductLineCode,
            name = product.Name,
            description = product.Description,
            requiresAttributeSelection =
                product.RequiresAttributeSelection,
            displayOrder = product.DisplayOrder,
            status = product.Status
        };
    }
}