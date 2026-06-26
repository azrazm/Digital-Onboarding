using System.ComponentModel.DataAnnotations;

namespace MerchantService.Api.Models;

public class CreateMerchantRequest
{
    // Excel field: Merchant Name
    // Field type: Mandatory
    [Required]
    public string MerchantName { get; set; } = string.Empty;

    // Excel field: Merchant VAT
    // Field type: Mandatory
    [Required]
    public string MerchantVat { get; set; } = string.Empty;

    // Excel field: Merchant Country
    // Field type: Mandatory
    [Required]
    [StringLength(2, MinimumLength = 2)]
    public string MerchantCountry { get; set; } = string.Empty;

    // Excel field: Service Country
    // Field type: Optional
    [StringLength(2, MinimumLength = 2)]
    public string? ServiceCountry { get; set; }

    // Excel field: Adress and Postal code
    // Field type: Mandatory
    [Required]
    public string AddressAndPostalCode { get; set; } = string.Empty;

    // Excel field: Industry Category 1
    // Field type: Mandatory
    [Required]
    public string IndustryCategory1 { get; set; } = string.Empty;

    // Excel field: Industry Category 2
    // Field type: Mandatory
    [Required]
    public string IndustryCategory2 { get; set; } = string.Empty;

    // Excel field: Industry Category 3
    // Field type: Optional
    public string? IndustryCategory3 { get; set; }

    // Excel field: Merchant Website
    // Field type: Optional
    public string? MerchantWebsite { get; set; }

    // Excel field: Merchant Group name
    // Field type: Optional
    public string? MerchantGroupName { get; set; }

    // Excel field: Merchant Type
    // Field type: Optional
    public string? MerchantType { get; set; }

    // Excel field: Legal Entity Identification Code
    // Field type: Optional
    public string? LegalEntityIdentificationCode { get; set; }

    // Excel field: Yearly Revenue in EUR
    // Field type: Mandatory
    [Required]
    public string YearlyRevenueInEur { get; set; } = string.Empty;
}
