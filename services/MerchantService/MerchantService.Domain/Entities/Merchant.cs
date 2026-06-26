namespace MerchantService.Domain.Entities;

public class Merchant
{
    // Interni identifikator Merchant Servicea.
    // Ne predstavlja direktno Excel polje "ID".
    public Guid Id { get; set; }

    // Excel field: Merchant Name
    // Field type: Mandatory
    public string MerchantName { get; set; } = string.Empty;

    // Excel field: Merchant VAT
    // Field type: Mandatory
    public string MerchantVat { get; set; } = string.Empty;

    // Excel field: Merchant Country
    // Field type: Mandatory
    public string MerchantCountry { get; set; } = string.Empty;

    // Excel field: Service Country
    // Field type: Optional
    public string? ServiceCountry { get; set; }

    // Excel field: Adress and Postal code
    // Field type: Mandatory
    // Napomena: naziv "Adress" je preuzet iz Excela.
    public string AddressAndPostalCode { get; set; } = string.Empty;

    // Excel field: Industry Category 1
    // Field type: Mandatory
    public string IndustryCategory1 { get; set; } = string.Empty;

    // Excel field: Industry Category 2
    // Field type: Mandatory
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
    // Za sada se čuva kao string jer Excel/UI koriste raspon vrijednosti.
    public string YearlyRevenueInEur { get; set; } = string.Empty;

    // Relacija prema kontakt osobama.
    // Nije zasebno Excel polje.
    public List<Contact> Contacts { get; set; } = [];
}
