namespace OnboardingApplicationService.Domain.Entities;

public class OnboardingApplication
{
    public Guid ApplicationId { get; set; }

    public string ApplicationNumber { get; set; } = string.Empty;

    public DateTime SubmittedAt { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public string? SubmittedByEmail { get; set; }

    public bool ConfirmedReview { get; set; }

    public SubmittedMerchant Merchant { get; set; } = new();

    public List<SubmittedContact> Contacts { get; set; } = new();

    public List<SubmittedProductSelection> ProductSelections { get; set; } = new();

    public SubmittedRequirements Requirements { get; set; } = new();

    public List<SubmittedDocument> Documents { get; set; } = new();
}

public class SubmittedMerchant
{
    public Guid? ExistingMerchantId { get; set; }

    public string MerchantName { get; set; } = string.Empty;

    public string MerchantVat { get; set; } = string.Empty;

    public string MerchantCountry { get; set; } = string.Empty;

    public string? ServiceCountry { get; set; }

    public string AddressAndPostalCode { get; set; } = string.Empty;

    public string IndustryCategory1 { get; set; } = string.Empty;

    public string IndustryCategory2 { get; set; } = string.Empty;

    public string? IndustryCategory3 { get; set; }

    public string? MerchantWebsite { get; set; }

    public string? MerchantGroupName { get; set; }

    public string? MerchantType { get; set; }

    public string? LegalEntityIdentificationCode { get; set; }

    public string YearlyRevenueInEur { get; set; } = string.Empty;
}

public class SubmittedContact
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string? Position { get; set; }

    public bool? IsDecisionMaker { get; set; }

    public string ContactType { get; set; } = string.Empty;
}

public class SubmittedProductSelection
{
    public string ProductLineCode { get; set; } = string.Empty;

    public string? ProductCode { get; set; }

    public List<string> AttributeCodes { get; set; } = new();
}

public class SubmittedRequirements
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

public class SubmittedDocument
{
    public Guid? DocumentId { get; set; } /*ovo ce mu vratiti Document Service*/

    public string DocumentType { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public string DocumentReference { get; set; } = string.Empty;
}