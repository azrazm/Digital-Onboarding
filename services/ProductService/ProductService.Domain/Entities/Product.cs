namespace ProductService.Domain.Entities;

public class Product
{
    public string ProductCode { get; set; } = string.Empty;

    public string ProductLineCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool RequiresAttributeSelection { get; set; }

    public int DisplayOrder { get; set; }

    public string Status { get; set; } = "Active";
}