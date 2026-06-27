using MerchantService.Api.Models.Requests;
using MerchantService.Application.Interfaces;
using MerchantService.Domain.Entities;
using MerchantService.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MerchantService.Api.Controllers;

[ApiController]
[Route("api/v1/merchants/{merchantId}/contacts")]
public class ContactsController : ControllerBase
{
    private readonly IMerchantRepository _merchantRepository;

    public ContactsController(
        IMerchantRepository merchantRepository)
    {
        _merchantRepository = merchantRepository;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMerchantContacts(
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

        var contacts =
            await _merchantRepository.GetContactsAsync(parsedMerchantId);

        if (contacts is null)
        {
            return NotFound(new
            {
                status = 404,
                message = "Merchant nije pronađen.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        var items = contacts
            .Select(MapContactDetails)
            .ToList();

        return Ok(new
        {
            items,
            totalCount = items.Count
        });
    }

    [HttpGet("{contactId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMerchantContactById(
        string merchantId,
        string contactId)
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

        if (!Guid.TryParse(contactId, out var parsedContactId))
        {
            return BadRequest(new
            {
                status = 400,
                message = "Contact ID nije u ispravnom UUID formatu.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        var contact =
            await _merchantRepository.GetContactByIdAsync(
                parsedMerchantId,
                parsedContactId);

        if (contact is null)
        {
            return NotFound(new
            {
                status = 404,
                message = "Merchant ili kontakt osoba nisu pronađeni.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        return Ok(MapContactDetails(contact));
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SaveMerchantContacts(
        string merchantId,
        [FromBody] SaveMerchantContactsRequest request)
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

        var hasDuplicateEmails = request.Contacts
            .GroupBy(contact =>
                contact.Email.Trim().ToLowerInvariant())
            .Any(group => group.Count() > 1);

        if (hasDuplicateEmails)
        {
            return Conflict(new
            {
                status = 409,
                message = "Kontakt osobe ne mogu imati isti email.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        var domainContacts = new List<Contact>();

        foreach (var contactRequest in request.Contacts)
        {
            if (!Enum.TryParse<ContactType>(
                    contactRequest.ContactType,
                    ignoreCase: true,
                    out var parsedContactType))
            {
                return BadRequest(new
                {
                    status = 400,
                    message = "ContactType mora biti Primary ili Secondary.",
                    traceId = HttpContext.TraceIdentifier
                });
            }

            domainContacts.Add(new Contact
            {
                Id = Guid.NewGuid(),
                MerchantId = parsedMerchantId,
                FirstName = contactRequest.FirstName.Trim(),
                LastName = contactRequest.LastName.Trim(),
                Email = contactRequest.Email.Trim(),
                PhoneNumber = contactRequest.PhoneNumber.Trim(),
                Position = contactRequest.Position?.Trim(),
                IsDecisionMaker = contactRequest.IsDecisionMaker,
                ContactType = parsedContactType
            });
        }

        var primaryContactsCount = domainContacts.Count(
            contact => contact.ContactType == ContactType.Primary);

        if (primaryContactsCount != 1)
        {
            return Conflict(new
            {
                status = 409,
                message = "Mora postojati tačno jedan primarni kontakt.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        var savedContacts =
            await _merchantRepository.SaveContactsAsync(
                parsedMerchantId,
                domainContacts);

        if (savedContacts is null)
        {
            return NotFound(new
            {
                status = 404,
                message = "Merchant nije pronađen.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        var items = savedContacts
            .Select(MapContactDetails)
            .ToList();

        return Ok(new
        {
            items,
            totalCount = items.Count
        });
    }

    private static object MapContactDetails(Contact contact)
    {
        return new
        {
            id = contact.Id,
            merchantId = contact.MerchantId,
            firstName = contact.FirstName,
            lastName = contact.LastName,
            email = contact.Email,
            phoneNumber = contact.PhoneNumber,
            position = contact.Position,
            isDecisionMaker = contact.IsDecisionMaker,
            contactType = contact.ContactType.ToString()
        };
    }
}