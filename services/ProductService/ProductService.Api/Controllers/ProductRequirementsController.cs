using ProductService.Api.Models.Requests;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ProductService.Api.Controllers;

[ApiController]
[Route("api/v1/product-requirements")]
public class ProductRequirementsController : ControllerBase
{
    private readonly IProductConfigurationRepository _repository;

    public ProductRequirementsController(
        IProductConfigurationRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("resolve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResolveProductRequirements(
        [FromBody] ResolveProductRequirementsRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.MerchantCountry)
            && request.MerchantCountry.Trim().Length != 2)
        {
            return BadRequest(new
            {
                status = 400,
                message = "MerchantCountry mora biti ISO kod od 2 znaka.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        var selectedProductLineCodes = request.Selections
            .Select(selection => selection.ProductLineCode.Trim())
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var selectedProductCodes = request.Selections
            .Where(selection =>
                !string.IsNullOrWhiteSpace(selection.ProductCode))
            .Select(selection => selection.ProductCode!.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var selectedAttributeCodes = request.Selections
            .SelectMany(selection => selection.AttributeCodes)
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Select(code => code.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var requirements =
            await _repository.ResolveRequirementsAsync(
                selectedProductLineCodes,
                selectedProductCodes,
                selectedAttributeCodes);

        var items = requirements
            .Select(MapRequirement)
            .ToList();

        return Ok(new
        {
            items,
            totalCount = items.Count
        });
    }

    private static object MapRequirement(
        ProductRequirement requirement)
    {
        return new
        {
            requirementCode = requirement.RequirementCode,
            label = requirement.Label,
            description = requirement.Description,
            fieldType = requirement.FieldType,
            isRequired = requirement.IsRequired,
            options = requirement.Options?.Select(option => new
            {
                code = option.Code,
                label = option.Label
            }),
            dependsOn = new
            {
                productLineCodes =
                    requirement.DependsOn.ProductLineCodes,
                productCodes =
                    requirement.DependsOn.ProductCodes,
                attributeCodes =
                    requirement.DependsOn.AttributeCodes
            },
            displayOrder = requirement.DisplayOrder,
            submitFieldName=requirement.SubmitFieldName
        };
    }
}