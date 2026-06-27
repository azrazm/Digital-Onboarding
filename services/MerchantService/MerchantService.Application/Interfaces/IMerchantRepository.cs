using MerchantService.Domain.Entities;

namespace MerchantService.Application.Interfaces;

public interface IMerchantRepository
{
    Task<IReadOnlyList<Merchant>> SearchAsync(
        string query,
        string country);

    Task<Merchant?> GetByIdAsync(Guid merchantId);

    Task<Merchant?> GetByVatAsync(string merchantVat);

    Task CreateAsync(Merchant merchant);

    Task<bool> UpdateAsync(Merchant merchant);

 /*Kontakt rute*/
    Task<IReadOnlyList<Contact>?> GetContactsAsync(Guid merchantId);

Task<Contact?> GetContactByIdAsync(
    Guid merchantId,
    Guid contactId);

Task<IReadOnlyList<Contact>?> SaveContactsAsync(
    Guid merchantId,
    IReadOnlyList<Contact> contacts);
}