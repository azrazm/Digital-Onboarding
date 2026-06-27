namespace ProductService.Domain.Entities;

public class ProductRequirement
{
    public string RequirementCode { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string FieldType { get; set; } = string.Empty;

    public bool IsRequired { get; set; }

    public List<RequirementOption>? Options { get; set; }

    public RequirementDependency DependsOn { get; set; } = new();

    public int DisplayOrder { get; set; }
}