using System.ComponentModel.DataAnnotations;

namespace MerchantService.Api.Models.Requests;

public class SaveMerchantContactsRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(2)]
    public List<ContactInputRequest> Contacts { get; set; } = new();
}

public class ContactInputRequest
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string PhoneNumber { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Position { get; set; }

    public bool? IsDecisionMaker { get; set; }

    [Required]
    public string ContactType { get; set; } = string.Empty;
}