using System.Text.Json;
using MerchantService.Application.Interfaces;
using MerchantService.Domain.Entities;
using System.Text.Json.Serialization;

namespace MerchantService.Infrastructure.Repositories;

public sealed class JsonMerchantRepository : IMerchantRepository
{
    private readonly string _filePath;

    public JsonMerchantRepository(string filePath)
{
    _filePath = filePath;
}
private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    public async Task<IReadOnlyList<Merchant>> SearchAsync(
        string query,
        string country)
    {
        var merchants = await ReadAllAsync();

        return merchants
            .Where(merchant =>
                string.Equals(
                    merchant.MerchantCountry,
                    country,
                    StringComparison.OrdinalIgnoreCase)
                &&
                (
                    merchant.MerchantName.Contains(
                        query,
                        StringComparison.OrdinalIgnoreCase)
                    ||
                    merchant.MerchantVat.Contains(
                        query,
                        StringComparison.OrdinalIgnoreCase)
                ))
            .ToList();
    }

    public async Task<Merchant?> GetByIdAsync(Guid merchantId)
    {
        var merchants = await ReadAllAsync();

        return merchants.FirstOrDefault(
            merchant => merchant.Id == merchantId);
    }

    public async Task<Merchant?> GetByVatAsync(string merchantVat)
    {
        var merchants = await ReadAllAsync();

        return merchants.FirstOrDefault(
            merchant => string.Equals(
                merchant.MerchantVat,
                merchantVat,
                StringComparison.OrdinalIgnoreCase));
    }

    public async Task CreateAsync(Merchant merchant)
    {
        var merchants = await ReadAllAsync();

        merchants.Add(merchant);

        await SaveAllAsync(merchants);
    }

    public async Task<bool> UpdateAsync(Merchant merchant)
    {
        var merchants = await ReadAllAsync();

        var merchantIndex = merchants.FindIndex(
            existingMerchant =>
                existingMerchant.Id == merchant.Id);

        if (merchantIndex == -1)
        {
            return false;
        }

        merchants[merchantIndex] = merchant;

        await SaveAllAsync(merchants);

        return true;
    }

    private async Task<List<Merchant>> ReadAllAsync()
    {
        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException(
                $"JSON fajl nije pronađen na lokaciji: {_filePath}");
        }

        await using var stream = File.OpenRead(_filePath);

        var merchants =
            await JsonSerializer.DeserializeAsync<List<Merchant>>(
                stream,
                _jsonOptions);

        return merchants ?? new List<Merchant>();
    }

    private async Task SaveAllAsync(List<Merchant> merchants)
    {
        var directory = Path.GetDirectoryName(_filePath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(_filePath);

        await JsonSerializer.SerializeAsync(
            stream,
            merchants,
            _jsonOptions);
    }
    public async Task<IReadOnlyList<Contact>?> GetContactsAsync(
    Guid merchantId)
{
    var merchant = await GetByIdAsync(merchantId);

    if (merchant is null)
    {
        return null;
    }

    return merchant.Contacts ?? new List<Contact>();
}

public async Task<Contact?> GetContactByIdAsync(
    Guid merchantId,
    Guid contactId)
{
    var merchant = await GetByIdAsync(merchantId);

    if (merchant is null)
    {
        return null;
    }

    return merchant.Contacts?
        .FirstOrDefault(contact => contact.Id == contactId);
}

public async Task<IReadOnlyList<Contact>?> SaveContactsAsync(
    Guid merchantId,
    IReadOnlyList<Contact> contacts)
{
    var merchants = await ReadAllAsync();

    var merchant = merchants.FirstOrDefault(
        existingMerchant => existingMerchant.Id == merchantId);

    if (merchant is null)
    {
        return null;
    }

    foreach (var contact in contacts)
    {
        contact.MerchantId = merchantId;
    }

    merchant.Contacts = contacts.ToList();

    await SaveAllAsync(merchants);

    return merchant.Contacts;
}
}