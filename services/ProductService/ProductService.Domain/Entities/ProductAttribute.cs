namespace ProductService.Domain.Entities;

public class ProductAttribute
{
    public string AttributeCode { get; set; } = string.Empty;

    public string ProductCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsRequired { get; set; }

    public int DisplayOrder { get; set; }

    public string Status { get; set; } = "Active";
}