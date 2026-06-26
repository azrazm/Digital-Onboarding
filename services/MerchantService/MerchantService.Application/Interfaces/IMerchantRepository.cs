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
}