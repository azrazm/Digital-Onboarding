using OnboardingApplicationService.Api.Models.Requests;
using OnboardingApplicationService.Application.Interfaces;
using OnboardingApplicationService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace OnboardingApplicationService.Api.Controllers;

[ApiController]
[Route("api/v1/onboarding-applications")]
public class OnboardingApplicationsController : ControllerBase
{
    private readonly IOnboardingApplicationRepository _repository;

    public OnboardingApplicationsController(
        IOnboardingApplicationRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("submit")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SubmitOnboardingApplication(
        [FromBody] SubmitOnboardingApplicationRequest request)
    {
        if (!request.ConfirmedReview)
        {
            return Conflict(new
            {
                status = 409,
                message = "Korisnik mora potvrditi da je pregledao podatke prije slanja.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        var duplicateEmails = request.Contacts
            .GroupBy(contact => contact.Email.Trim().ToLowerInvariant())
            .Any(group => group.Count() > 1);

        if (duplicateEmails)
        {
            return Conflict(new
            {
                status = 409,
                message = "Kontakt osobe ne mogu imati isti email.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        var primaryContactsCount = request.Contacts.Count(
            contact => string.Equals(
                contact.ContactType.Trim(),
                "Primary",
                StringComparison.OrdinalIgnoreCase));

        if (primaryContactsCount != 1)
        {
            return Conflict(new
            {
                status = 409,
                message = "Mora postojati tačno jedan primarni kontakt.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        var hasInvalidContactType = request.Contacts.Any(
            contact =>
                !string.Equals(
                    contact.ContactType.Trim(),
                    "Primary",
                    StringComparison.OrdinalIgnoreCase)
                &&
                !string.Equals(
                    contact.ContactType.Trim(),
                    "Secondary",
                    StringComparison.OrdinalIgnoreCase));

        if (hasInvalidContactType)
        {
            return BadRequest(new
            {
                status = 400,
                message = "ContactType mora biti Primary ili Secondary.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        var existingApplicationsCount =
            await _repository.CountAsync();

        var now = DateTime.UtcNow;

        var application = new OnboardingApplication
        {
            ApplicationId = Guid.NewGuid(),
            ApplicationNumber =
                $"ONB-{now.Year}-{existingApplicationsCount + 1:000000}",
            SubmittedAt = now,
            LastUpdatedAt = now,
            SubmittedByEmail = request.SubmittedByEmail?.Trim(),
            ConfirmedReview = request.ConfirmedReview,
            Merchant = MapMerchant(request.Merchant),
            Contacts = request.Contacts
                .Select(MapContact)
                .ToList(),
            ProductSelections = request.ProductSelections
                .Select(MapProductSelection)
                .ToList(),
            Requirements = MapRequirements(request.Requirements),
            Documents = request.Documents
                .Select(MapDocument)
                .ToList()
        };

        await _repository.CreateAsync(application);

        return CreatedAtAction(
            nameof(GetOnboardingApplicationById),
            new { applicationId = application.ApplicationId },
            MapApplicationDetails(application));
    }

/*ruta vezana za internu logiku, da dobavimo sve submitane applications*/
[HttpGet]
[ProducesResponseType(StatusCodes.Status200OK)]
public async Task<IActionResult> GetOnboardingApplications()
{
    var applications = await _repository.GetAllAsync();

    var items = applications
        .Select(MapApplicationSummary)
        .ToList();

    return Ok(new
    {
        items,
        totalCount = items.Count
    });
}

    [HttpGet("{applicationId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOnboardingApplicationById(
        string applicationId)
    {
        if (!Guid.TryParse(applicationId, out var parsedApplicationId))
        {
            return BadRequest(new
            {
                status = 400,
                message = "Application ID nije u ispravnom UUID formatu.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        var application =
            await _repository.GetByIdAsync(parsedApplicationId);

        if (application is null)
        {
            return NotFound(new
            {
                status = 404,
                message = "Onboarding prijava nije pronađena.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        return Ok(MapApplicationDetails(application));
    }

    private static SubmittedMerchant MapMerchant(
        SubmittedMerchantRequest merchant)
    {
        return new SubmittedMerchant
        {
            ExistingMerchantId = merchant.ExistingMerchantId,
            MerchantName = merchant.MerchantName.Trim(),
            MerchantVat = merchant.MerchantVat.Trim(),
            MerchantCountry =
                merchant.MerchantCountry.Trim().ToUpperInvariant(),
            ServiceCountry =
                merchant.ServiceCountry?.Trim().ToUpperInvariant(),
            AddressAndPostalCode =
                merchant.AddressAndPostalCode.Trim(),
            IndustryCategory1 =
                merchant.IndustryCategory1.Trim(),
            IndustryCategory2 =
                merchant.IndustryCategory2.Trim(),
            IndustryCategory3 =
                merchant.IndustryCategory3?.Trim(),
            MerchantWebsite =
                merchant.MerchantWebsite?.Trim(),
            MerchantGroupName =
                merchant.MerchantGroupName?.Trim(),
            MerchantType =
                merchant.MerchantType?.Trim(),
            LegalEntityIdentificationCode =
                merchant.LegalEntityIdentificationCode?.Trim(),
            YearlyRevenueInEur =
                merchant.YearlyRevenueInEur.Trim()
        };
    }

    private static SubmittedContact MapContact(
        SubmittedContactRequest contact)
    {
        return new SubmittedContact
        {
            FirstName = contact.FirstName.Trim(),
            LastName = contact.LastName.Trim(),
            Email = contact.Email.Trim(),
            PhoneNumber = contact.PhoneNumber.Trim(),
            Position = contact.Position?.Trim(),
            IsDecisionMaker = contact.IsDecisionMaker,
            ContactType = NormalizeContactType(contact.ContactType)
        };
    }

    private static SubmittedProductSelection MapProductSelection(
        SubmittedProductSelectionRequest selection)
    {
        return new SubmittedProductSelection
        {
            ProductLineCode =
                selection.ProductLineCode.Trim().ToUpperInvariant(),
            ProductCode =
                selection.ProductCode?.Trim().ToUpperInvariant(),
            AttributeCodes = selection.AttributeCodes
                .Select(code => code.Trim().ToUpperInvariant())
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .ToList()
        };
    }

    private static SubmittedRequirements MapRequirements(
        SubmittedRequirementsRequest requirements)
    {
        return new SubmittedRequirements
        {
            YearlyCardTransactionNumber =
                requirements.YearlyCardTransactionNumber,
            YearlyCardTransactionVolumeInEur =
                requirements.YearlyCardTransactionVolumeInEur,
            PartnersAndDistributors =
                requirements.PartnersAndDistributors?.Trim(),
            IntegratorsAndSystemProviders =
                requirements.IntegratorsAndSystemProviders?.Trim(),
            NumberOfLocations =
                requirements.NumberOfLocations,
            NumberOfDevices =
                requirements.NumberOfDevices,
            NumberOfLicensesPaid =
                requirements.NumberOfLicensesPaid,
            NumberOfLicensesTotal =
                requirements.NumberOfLicensesTotal,
            ModelOfDeviceHw =
                requirements.ModelOfDeviceHw?.Trim(),
            MainBank =
                requirements.MainBank?.Trim(),
            Gdpr =
                requirements.Gdpr
        };
    }

    private static SubmittedDocument MapDocument(
        SubmittedDocumentRequest document)
    {
        return new SubmittedDocument
        {
            DocumentType =
                document.DocumentType.Trim().ToUpperInvariant(),
            FileName =
                document.FileName.Trim(),
            DocumentReference =
                document.DocumentReference?.Trim()
        };
    }

    private static string NormalizeContactType(string contactType)
    {
        if (string.Equals(
                contactType.Trim(),
                "Primary",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Primary";
        }

        return "Secondary";
    }

    private static object MapApplicationDetails(
        OnboardingApplication application)
    {
        return new
        {
            applicationId = application.ApplicationId,
            applicationNumber = application.ApplicationNumber,
            submittedAt = application.SubmittedAt,
            lastUpdatedAt = application.LastUpdatedAt,
            submittedByEmail = application.SubmittedByEmail,
            confirmedReview = application.ConfirmedReview,
            merchant = application.Merchant,
            contacts = application.Contacts,
            productSelections = application.ProductSelections,
            requirements = application.Requirements,
            documents = application.Documents
        };
    }

    private static object MapApplicationSummary(
    OnboardingApplication application)
{
    var primaryContact = application.Contacts.FirstOrDefault(
        contact => string.Equals(
            contact.ContactType,
            "Primary",
            StringComparison.OrdinalIgnoreCase));

    var firstContact = primaryContact ?? application.Contacts.FirstOrDefault();

    var productLineCodes = application.ProductSelections
        .Select(selection => selection.ProductLineCode)
        .Distinct()
        .ToList();

    return new
    {
        applicationId = application.ApplicationId,
        applicationNumber = application.ApplicationNumber,
        merchantName = application.Merchant.MerchantName,
        merchantVat = application.Merchant.MerchantVat,
        merchantCountry = application.Merchant.MerchantCountry,
        submittedAt = application.SubmittedAt,
        submittedByEmail = application.SubmittedByEmail,
        primaryContactEmail = firstContact?.Email,
        productLineCodes,
        documentsCount = application.Documents.Count
    };
}
}