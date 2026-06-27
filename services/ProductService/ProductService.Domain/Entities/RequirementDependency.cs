namespace ProductService.Domain.Entities;

public class RequirementDependency
{
    public List<string> ProductLineCodes { get; set; } = new();

    public List<string> ProductCodes { get; set; } = new();

    public List<string> AttributeCodes { get; set; } = new();
}