using System.Text.Json;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Repositories;

public sealed class JsonProductConfigurationRepository
    : IProductConfigurationRepository
{
    private readonly string _filePath;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public JsonProductConfigurationRepository(string filePath)
    {
        _filePath = filePath;
    }

    public async Task<IReadOnlyList<ProductLine>> GetProductLinesAsync()
    {
        var data = await ReadConfigurationAsync();

        return data.ProductLines
            .OrderBy(productLine => productLine.DisplayOrder)
            .ToList();
    }

    public async Task<ProductLine?> GetProductLineByCodeAsync(
        string productLineCode)
    {
        var data = await ReadConfigurationAsync();

        return data.ProductLines.FirstOrDefault(
            productLine => string.Equals(
                productLine.ProductLineCode,
                productLineCode,
                StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IReadOnlyList<Product>> GetProductsByProductLineAsync(
        string productLineCode)
    {
        var data = await ReadConfigurationAsync();

        return data.Products
            .Where(product => string.Equals(
                product.ProductLineCode,
                productLineCode,
                StringComparison.OrdinalIgnoreCase))
            .OrderBy(product => product.DisplayOrder)
            .ToList();
    }

    public async Task<Product?> GetProductByCodeAsync(
        string productCode)
    {
        var data = await ReadConfigurationAsync();

        return data.Products.FirstOrDefault(
            product => string.Equals(
                product.ProductCode,
                productCode,
                StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IReadOnlyList<ProductAttribute>> GetAttributesByProductCodeAsync(
        string productCode)
    {
        var data = await ReadConfigurationAsync();

        return data.Attributes
            .Where(attribute => string.Equals(
                attribute.ProductCode,
                productCode,
                StringComparison.OrdinalIgnoreCase))
            .OrderBy(attribute => attribute.DisplayOrder)
            .ToList();
    }

    public async Task<IReadOnlyList<ProductRequirement>> ResolveRequirementsAsync(
        IReadOnlySet<string> selectedProductLineCodes,
        IReadOnlySet<string> selectedProductCodes,
        IReadOnlySet<string> selectedAttributeCodes)
    {
        var data = await ReadConfigurationAsync();

        return data.Requirements
            .Where(requirement => DependencyMatches(
                requirement.DependsOn,
                selectedProductLineCodes,
                selectedProductCodes,
                selectedAttributeCodes))
            .OrderBy(requirement => requirement.DisplayOrder)
            .ToList();
    }

    private async Task<ProductConfigurationData> ReadConfigurationAsync()
    {
        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException(
                $"Product configuration JSON nije pronađen: {_filePath}");
        }

        await using var stream = File.OpenRead(_filePath);

        var data =
            await JsonSerializer.DeserializeAsync<ProductConfigurationData>(
                stream,
                _jsonOptions);

        return data ?? new ProductConfigurationData();
    }

    private static bool DependencyMatches(
        RequirementDependency dependency,
        IReadOnlySet<string> selectedProductLineCodes,
        IReadOnlySet<string> selectedProductCodes,
        IReadOnlySet<string> selectedAttributeCodes)
    {
        return Matches(
                dependency.ProductLineCodes,
                selectedProductLineCodes)
            && Matches(
                dependency.ProductCodes,
                selectedProductCodes)
            && Matches(
                dependency.AttributeCodes,
                selectedAttributeCodes);
    }

    private static bool Matches(
        IReadOnlyCollection<string> requiredCodes,
        IReadOnlySet<string> selectedCodes)
    {
        if (requiredCodes.Count == 0)
        {
            return true;
        }

        return requiredCodes.Any(selectedCodes.Contains);
    }

    private sealed class ProductConfigurationData
    {
        public List<ProductLine> ProductLines { get; set; } = new();

        public List<Product> Products { get; set; } = new();

        public List<ProductAttribute> Attributes { get; set; } = new();

        public List<ProductRequirement> Requirements { get; set; } = new();
    }
}