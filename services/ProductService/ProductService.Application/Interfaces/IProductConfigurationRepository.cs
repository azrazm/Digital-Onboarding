using ProductService.Domain.Entities;

namespace ProductService.Application.Interfaces;

public interface IProductConfigurationRepository
{
    Task<IReadOnlyList<ProductLine>> GetProductLinesAsync();

    Task<ProductLine?> GetProductLineByCodeAsync(
        string productLineCode);

    Task<IReadOnlyList<Product>> GetProductsByProductLineAsync(
        string productLineCode);

    Task<Product?> GetProductByCodeAsync(
        string productCode);

    Task<IReadOnlyList<ProductAttribute>> GetAttributesByProductCodeAsync(
        string productCode);

    Task<IReadOnlyList<ProductRequirement>> ResolveRequirementsAsync(
        IReadOnlySet<string> selectedProductLineCodes,
        IReadOnlySet<string> selectedProductCodes,
        IReadOnlySet<string> selectedAttributeCodes);
}