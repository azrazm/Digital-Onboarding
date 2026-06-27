using Microsoft.AspNetCore.Mvc;
using MerchantService.Api.Models;
using MerchantService.Api.Models.Requests;
using MerchantService.Application.Interfaces;
using MerchantService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
namespace MerchantService.Api.Controllers;

[ApiController]
[Route("api/v1/merchants")]
public class MerchantsController : ControllerBase
{
    private readonly IMerchantRepository _merchantRepository;

public MerchantsController(
    IMerchantRepository merchantRepository)
{
    _merchantRepository = merchantRepository;
}
    [HttpGet("search")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<IActionResult> SearchMerchants(
    [FromQuery] string query,
    [FromQuery] string country)
{
    if (string.IsNullOrWhiteSpace(query)
        || query.Trim().Length < 2)
    {
        return BadRequest(new
        {
            status = 400,
            message = "Parametar query mora sadržavati najmanje 2 znaka.",
            traceId = HttpContext.TraceIdentifier
        });
    }

    if (string.IsNullOrWhiteSpace(country)
        || country.Trim().Length != 2)
    {
        return BadRequest(new
        {
            status = 400,
            message = "Country mora biti ISO kod od 2 znaka.",
            traceId = HttpContext.TraceIdentifier
        });
    }

    var merchants = await _merchantRepository.SearchAsync(
        query.Trim(),
        country.Trim());

    var items = merchants.Select(merchant => new
    {
        id = merchant.Id,
        merchantName = merchant.MerchantName,
        merchantVat = merchant.MerchantVat,
        merchantCountry = merchant.MerchantCountry,
        addressAndPostalCode = merchant.AddressAndPostalCode
    }).ToList();

    return Ok(new
    {
        items,
        totalCount = items.Count
    });
}

[HttpGet("{merchantId}")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> GetMerchantById(
    string merchantId)
{
    if (!Guid.TryParse(merchantId, out var parsedMerchantId))
    {
        return BadRequest(new
        {
            status = 400,
            message = "Merchant ID nije u ispravnom UUID formatu.",
            traceId = HttpContext.TraceIdentifier
        });
    }

    var merchant =
        await _merchantRepository.GetByIdAsync(parsedMerchantId);

    if (merchant is null)
    {
        return NotFound(new
        {
            status = 404,
            message = "Merchant nije pronađen.",
            traceId = HttpContext.TraceIdentifier
        });
    }

    return Ok(new
    {
        id = merchant.Id,
        merchantName = merchant.MerchantName,
        merchantVat = merchant.MerchantVat,
        merchantCountry = merchant.MerchantCountry,
        serviceCountry = merchant.ServiceCountry,
        addressAndPostalCode = merchant.AddressAndPostalCode,
        industryCategory1 = merchant.IndustryCategory1,
        industryCategory2 = merchant.IndustryCategory2,
        industryCategory3 = merchant.IndustryCategory3,
        merchantWebsite = merchant.MerchantWebsite,
        merchantGroupName = merchant.MerchantGroupName,
        merchantType = merchant.MerchantType,
        legalEntityIdentificationCode =
            merchant.LegalEntityIdentificationCode,
        yearlyRevenueInEur = merchant.YearlyRevenueInEur
    });
}

[HttpPost]
[ProducesResponseType(StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status409Conflict)]
public async Task<IActionResult> CreateMerchant(
    [FromBody] CreateMerchantRequest request)
{
    var merchantWithSameVat =
        await _merchantRepository.GetByVatAsync(
            request.MerchantVat.Trim());

    if (merchantWithSameVat is not null)
    {
        return Conflict(new
        {
            status = 409,
            message =
                "Merchant s istim poreznim identifikacijskim brojem već postoji.",
            traceId = HttpContext.TraceIdentifier
        });
    }

    var merchant = new Merchant
    {
        Id = Guid.NewGuid(),
        MerchantName = request.MerchantName.Trim(),
        MerchantVat = request.MerchantVat.Trim(),
        MerchantCountry =
            request.MerchantCountry.Trim().ToUpperInvariant(),
        ServiceCountry =
            request.ServiceCountry?.Trim().ToUpperInvariant(),
        AddressAndPostalCode =
            request.AddressAndPostalCode.Trim(),
        IndustryCategory1 =
            request.IndustryCategory1.Trim(),
        IndustryCategory2 =
            request.IndustryCategory2.Trim(),
        IndustryCategory3 =
            request.IndustryCategory3?.Trim(),
        MerchantWebsite =
            request.MerchantWebsite?.Trim(),
        MerchantGroupName =
            request.MerchantGroupName?.Trim(),
        MerchantType =
            request.MerchantType?.Trim(),
        LegalEntityIdentificationCode =
            request.LegalEntityIdentificationCode?.Trim(),
        YearlyRevenueInEur =
            request.YearlyRevenueInEur.Trim()
    };

    await _merchantRepository.CreateAsync(merchant);

    var response = MapMerchantDetails(merchant);

    return CreatedAtAction(
        nameof(GetMerchantById),
        new { merchantId = merchant.Id },
        response);
}

[HttpPut("{merchantId}")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status409Conflict)]
public async Task<IActionResult> UpdateMerchant(
    string merchantId,
    [FromBody] UpdateMerchantRequest request)
{
    if (!Guid.TryParse(merchantId, out var parsedMerchantId))
    {
        return BadRequest(new
        {
            status = 400,
            message =
                "Merchant ID nije u ispravnom UUID formatu.",
            traceId = HttpContext.TraceIdentifier
        });
    }

    var existingMerchant =
        await _merchantRepository.GetByIdAsync(parsedMerchantId);

    if (existingMerchant is null)
    {
        return NotFound(new
        {
            status = 404,
            message = "Merchant nije pronađen.",
            traceId = HttpContext.TraceIdentifier
        });
    }

    var merchantWithSameVat =
        await _merchantRepository.GetByVatAsync(
            request.MerchantVat.Trim());

    if (merchantWithSameVat is not null
        && merchantWithSameVat.Id != parsedMerchantId)
    {
        return Conflict(new
        {
            status = 409,
            message =
                "Porezni identifikacijski broj pripada drugom merchantu.",
            traceId = HttpContext.TraceIdentifier
        });
    }

    existingMerchant.MerchantName =
        request.MerchantName.Trim();

    existingMerchant.MerchantVat =
        request.MerchantVat.Trim();

    existingMerchant.MerchantCountry =
        request.MerchantCountry.Trim().ToUpperInvariant();

    existingMerchant.ServiceCountry =
        request.ServiceCountry?.Trim().ToUpperInvariant();

    existingMerchant.AddressAndPostalCode =
        request.AddressAndPostalCode.Trim();

    existingMerchant.IndustryCategory1 =
        request.IndustryCategory1.Trim();

    existingMerchant.IndustryCategory2 =
        request.IndustryCategory2.Trim();

    existingMerchant.IndustryCategory3 =
        request.IndustryCategory3?.Trim();

    existingMerchant.MerchantWebsite =
        request.MerchantWebsite?.Trim();

    existingMerchant.MerchantGroupName =
        request.MerchantGroupName?.Trim();

    existingMerchant.MerchantType =
        request.MerchantType?.Trim();

    existingMerchant.LegalEntityIdentificationCode =
        request.LegalEntityIdentificationCode?.Trim();

    existingMerchant.YearlyRevenueInEur =
        request.YearlyRevenueInEur.Trim();

    var updated =
        await _merchantRepository.UpdateAsync(existingMerchant);

    if (!updated)
    {
        return NotFound(new
        {
            status = 404,
            message = "Merchant nije pronađen.",
            traceId = HttpContext.TraceIdentifier
        });
    }

    return Ok(MapMerchantDetails(existingMerchant));
}

private static object MapMerchantDetails(Merchant merchant)
{
    return new
    {
        id = merchant.Id,
        merchantName = merchant.MerchantName,
        merchantVat = merchant.MerchantVat,
        merchantCountry = merchant.MerchantCountry,
        serviceCountry = merchant.ServiceCountry,
        addressAndPostalCode = merchant.AddressAndPostalCode,
        industryCategory1 = merchant.IndustryCategory1,
        industryCategory2 = merchant.IndustryCategory2,
        industryCategory3 = merchant.IndustryCategory3,
        merchantWebsite = merchant.MerchantWebsite,
        merchantGroupName = merchant.MerchantGroupName,
        merchantType = merchant.MerchantType,
        legalEntityIdentificationCode =
            merchant.LegalEntityIdentificationCode,
        yearlyRevenueInEur = merchant.YearlyRevenueInEur
    };
}

}