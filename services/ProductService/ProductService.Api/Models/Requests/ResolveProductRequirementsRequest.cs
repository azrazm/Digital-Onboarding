using System.ComponentModel.DataAnnotations;

namespace ProductService.Api.Models.Requests;

public class ResolveProductRequirementsRequest
{
    [StringLength(2, MinimumLength = 2)]
    public string? MerchantCountry { get; set; }

    [Required]
    [MinLength(1)]
    public List<ProductSelectionRequest> Selections { get; set; } = new();
}

public class ProductSelectionRequest
{
    [Required]
    public string ProductLineCode { get; set; } = string.Empty;

    public string? ProductCode { get; set; }

    public List<string> AttributeCodes { get; set; } = new();
}