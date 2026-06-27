using System.ComponentModel.DataAnnotations;

namespace OnboardingApplicationService.Api.Models.Requests;

public class SubmitOnboardingApplicationRequest
{
    [Required]
    public SubmittedMerchantRequest Merchant { get; set; } = new();

    [Required]
    [MinLength(1)]
    [MaxLength(2)]
    public List<SubmittedContactRequest> Contacts { get; set; } = new();

    [Required]
    [MinLength(1)]
    public List<SubmittedProductSelectionRequest> ProductSelections { get; set; } = new();

    [Required]
    public SubmittedRequirementsRequest Requirements { get; set; } = new();

    public List<SubmittedDocumentRequest> Documents { get; set; } = new();

    [Required]
    public bool ConfirmedReview { get; set; }

    [EmailAddress]
    public string? SubmittedByEmail { get; set; }
}

public class SubmittedMerchantRequest
{
    public Guid? ExistingMerchantId { get; set; }

    [Required]
    public string MerchantName { get; set; } = string.Empty;

    [Required]
    public string MerchantVat { get; set; } = string.Empty;

    [Required]
    [StringLength(2, MinimumLength = 2)]
    public string MerchantCountry { get; set; } = string.Empty;

    [StringLength(2, MinimumLength = 2)]
    public string? ServiceCountry { get; set; }

    [Required]
    public string AddressAndPostalCode { get; set; } = string.Empty;

    [Required]
    public string IndustryCategory1 { get; set; } = string.Empty;

    [Required]
    public string IndustryCategory2 { get; set; } = string.Empty;

    public string? IndustryCategory3 { get; set; }

    [Url]
    public string? MerchantWebsite { get; set; }

    public string? MerchantGroupName { get; set; }

    public string? MerchantType { get; set; }

    public string? LegalEntityIdentificationCode { get; set; }

    [Required]
    public string YearlyRevenueInEur { get; set; } = string.Empty;
}

public class SubmittedContactRequest
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;

    public string? Position { get; set; }

    public bool? IsDecisionMaker { get; set; }

    [Required]
    public string ContactType { get; set; } = string.Empty;
}

public class SubmittedProductSelectionRequest
{
    [Required]
    public string ProductLineCode { get; set; } = string.Empty;

    public string? ProductCode { get; set; }

    public List<string> AttributeCodes { get; set; } = new();
}

public class SubmittedRequirementsRequest
{
    public int? YearlyCardTransactionNumber { get; set; }

    public decimal? YearlyCardTransactionVolumeInEur { get; set; }

    public string? PartnersAndDistributors { get; set; }

    public string? IntegratorsAndSystemProviders { get; set; }

    public int? NumberOfLocations { get; set; }

    public int? NumberOfDevices { get; set; }

    public int? NumberOfLicensesPaid { get; set; }

    public int? NumberOfLicensesTotal { get; set; }

    public string? ModelOfDeviceHw { get; set; }

    public string? MainBank { get; set; }

    public bool? Gdpr { get; set; }
}

public class SubmittedDocumentRequest
{
    [Required]
    public string DocumentType { get; set; } = string.Empty;

    [Required]
    public string FileName { get; set; } = string.Empty;

    public string? DocumentReference { get; set; }
}