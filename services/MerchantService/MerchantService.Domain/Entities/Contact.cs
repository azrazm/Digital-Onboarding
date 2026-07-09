namespace MerchantService.Domain.Entities;

public class Contact
{
    // Interni identifikator kontakta.
    // Nije direktno Excel polje.
    public Guid Id { get; set; }

    // Interna veza prema Merchant entitetu.
    // Nije direktno Excel polje.
    public Guid MerchantId { get; set; }

    // Excel field: Contact Person Name
    // Field type: Mandatory
    public string FirstName { get; set; } = string.Empty;

    // Excel field: Contact Person Surname
    // Field type: Mandatory
    public string LastName { get; set; } = string.Empty;

    // Excel field: Contact Person Email
    // Field type: Mandatory
    public string Email { get; set; } = string.Empty;

    // Excel field: Contact Person Ph.Number
    // Field type: Mandatory
    public string PhoneNumber { get; set; } = string.Empty;

    // Excel field: Position
    // Field type: Optional
    public string? Position { get; set; }

    // Excel field: Decision Maker
    // Field type: Optional
    //
    // true  = Da
    // false = Ne
    // null  = Nije odabrano
    public bool? IsDecisionMaker { get; set; }

    // Navigacijska veza prema Merchant entitetu.
    // Nije direktno Excel polje.
    public Merchant? Merchant { get; set; }
}