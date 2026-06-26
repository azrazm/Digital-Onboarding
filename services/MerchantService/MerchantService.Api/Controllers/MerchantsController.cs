using Microsoft.AspNetCore.Mvc;
using MerchantService.Api.Models;
using MerchantService.Api.Models.Requests;
namespace MerchantService.Api.Controllers;

[ApiController]
[Route("api/v1/merchants")]
public class MerchantsController : ControllerBase
{
    [HttpGet("search")]
    public IActionResult SearchMerchants(
        [FromQuery] string query,
        [FromQuery] string country)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
        {
            return BadRequest(new
            {
                status = 400,
                message = "Parametar query mora sadržavati najmanje 2 znaka."
            });
        }

        if (string.IsNullOrWhiteSpace(country) || country.Length != 2)
        {
            return BadRequest(new
            {
                status = 400,
                message = "Parametar country mora sadržavati ISO kod države od 2 znaka."
            });
        }

        var response = new
        {
            items = new[]
            {
                new
                {
                    id = Guid.Parse("7d05319f-7b89-4e99-9fab-a3a83961e74d"),
                    merchantName = "Monri Payments d.o.o.",
                    merchantVat = "27746883608",
                    merchantCountry = country.ToUpper(),
                    addressAndPostalCode = "Bani 75, 10010 Zagreb"
                }
            },
            totalCount = 1
        };

        return Ok(response);
    }


[HttpGet("{merchantId}")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public IActionResult GetMerchantById(string merchantId)
{
    // Provjerava je li poslani identifikator u ispravnom UUID formatu.
    if (!Guid.TryParse(merchantId, out var parsedMerchantId))
    {
        return BadRequest(new
        {
            status = 400,
            message = "Merchant ID nije u ispravnom UUID formatu."
        });
    }

    // Mock ID firme koji je vraćen iz search endpointa.
    var existingMerchantId =
        Guid.Parse("7d05319f-7b89-4e99-9fab-a3a83961e74d");

    // Simulacija situacije u kojoj merchant nije pronađen.
    if (parsedMerchantId != existingMerchantId)
    {
        return NotFound(new
        {
            status = 404,
            message = "Merchant nije pronađen."
        });
    }

    // Mock podaci koji simuliraju dohvat firme iz baze ili vanjskog sistema.
    var response = new
    {
        id = existingMerchantId,

        // Excel field: Merchant Name
        merchantName = "Monri Payments d.o.o.",

        // Excel field: Merchant VAT
        merchantVat = "27746883608",

        // Excel field: Merchant Country
        merchantCountry = "HR",

        // Excel field: Service Country
        serviceCountry = "HR",

        // Excel field: Adress and Postal code
        addressAndPostalCode = "Bani 75, 10010 Zagreb",

        // Excel field: Industry Category 1
        industryCategory1 = "Finance",

        // Excel field: Industry Category 2
        industryCategory2 = "Banking",

        // Excel field: Industry Category 3
        industryCategory3 = "Retail Banking",

        // Excel field: Merchant Website
        merchantWebsite = "https://monri.hr",

        // Excel field: Merchant Group name
        merchantGroupName = "Payten",

        // Excel field: Merchant Type
        merchantType = "B2B",

        // Excel field: Legal Entity Identification Code
        legalEntityIdentificationCode = "549300MONRI0000HR01",

        // Excel field: Yearly Revenue in EUR
        yearlyRevenueInEur = "10.000.000 – 50.000.000 EUR"
    };

    return Ok(response);
}

[HttpPost]
[ProducesResponseType(StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status409Conflict)]
public IActionResult CreateMerchant(
    [FromBody] CreateMerchantRequest request)
{
    // kreiranje novog internog ID-a.
    var merchantId = Guid.NewGuid();

    var response = new
    {
        id = merchantId,
        merchantName = request.MerchantName,
        merchantVat = request.MerchantVat,
        merchantCountry = request.MerchantCountry.ToUpperInvariant(),
        serviceCountry = request.ServiceCountry?.ToUpperInvariant(),
        addressAndPostalCode = request.AddressAndPostalCode,
        industryCategory1 = request.IndustryCategory1,
        industryCategory2 = request.IndustryCategory2,
        industryCategory3 = request.IndustryCategory3,
        merchantWebsite = request.MerchantWebsite,
        merchantGroupName = request.MerchantGroupName,
        merchantType = request.MerchantType,
        legalEntityIdentificationCode =
            request.LegalEntityIdentificationCode,
        yearlyRevenueInEur = request.YearlyRevenueInEur
    };

    return CreatedAtAction(
        nameof(GetMerchantById),
        new { merchantId },
        response
    );
}

[HttpPut("{merchantId}")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status409Conflict)]
public IActionResult UpdateMerchant(
    string merchantId,
    [FromBody] UpdateMerchantRequest request)
{
    // Provjera UUID formata.
    if (!Guid.TryParse(merchantId, out var parsedMerchantId))
    {
        return BadRequest(new
        {
            status = 400,
            message = "Merchant ID nije u ispravnom UUID formatu."
        });
    }

    // Mock merchant koji trenutno postoji u sistemu.
    var existingMerchantId =
        Guid.Parse("7d05319f-7b89-4e99-9fab-a3a83961e74d");

    if (parsedMerchantId != existingMerchantId)
    {
        return NotFound(new
        {
            status = 404,
            message = "Merchant nije pronađen."
        });
    }

    // Mock simulacija da drugi merchant već koristi ovaj porezni broj.
    if (request.MerchantVat == "12345678901")
    {
        return Conflict(new
        {
            status = 409,
            message = "Porezni identifikacijski broj pripada drugom merchantu."
        });
    }

    // Trenutno se podaci samo vraćaju kao da su ažurirani.
    // Ne spremaju se trajno jer još nemamo bazu ili JSON repository.
    var response = new
    {
        id = existingMerchantId,
        merchantName = request.MerchantName,
        merchantVat = request.MerchantVat,
        merchantCountry = request.MerchantCountry.ToUpperInvariant(),
        serviceCountry = request.ServiceCountry?.ToUpperInvariant(),
        addressAndPostalCode = request.AddressAndPostalCode,
        industryCategory1 = request.IndustryCategory1,
        industryCategory2 = request.IndustryCategory2,
        industryCategory3 = request.IndustryCategory3,
        merchantWebsite = request.MerchantWebsite,
        merchantGroupName = request.MerchantGroupName,
        merchantType = request.MerchantType,
        legalEntityIdentificationCode =
            request.LegalEntityIdentificationCode,
        yearlyRevenueInEur = request.YearlyRevenueInEur
    };

    return Ok(response);
}

}