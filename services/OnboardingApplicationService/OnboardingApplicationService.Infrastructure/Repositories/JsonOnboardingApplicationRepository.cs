using System.Text.Json;
using OnboardingApplicationService.Application.Interfaces;
using OnboardingApplicationService.Domain.Entities;

namespace OnboardingApplicationService.Infrastructure.Repositories;

public sealed class JsonOnboardingApplicationRepository
    : IOnboardingApplicationRepository
{
    private readonly string _filePath;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public JsonOnboardingApplicationRepository(string filePath)
    {
        _filePath = filePath;
    }

    public async Task CreateAsync(OnboardingApplication application)
    {
        var applications = await ReadAllAsync();

        applications.Add(application);

        await SaveAllAsync(applications);
    }

    public async Task<OnboardingApplication?> GetByIdAsync(
        Guid applicationId)
    {
        var applications = await ReadAllAsync();

        return applications.FirstOrDefault(
            application => application.ApplicationId == applicationId);
    }

    public async Task<int> CountAsync()
    {
        var applications = await ReadAllAsync();

        return applications.Count;
    }

    private async Task<List<OnboardingApplication>> ReadAllAsync()
    {
        if (!File.Exists(_filePath))
        {
            return new List<OnboardingApplication>();
        }

        await using var stream = File.OpenRead(_filePath);

        var applications =
            await JsonSerializer.DeserializeAsync<List<OnboardingApplication>>(
                stream,
                _jsonOptions);

        return applications ?? new List<OnboardingApplication>();
    }

    private async Task SaveAllAsync(
        List<OnboardingApplication> applications)
    {
        var directory = Path.GetDirectoryName(_filePath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(_filePath);

        await JsonSerializer.SerializeAsync(
            stream,
            applications,
            _jsonOptions);
    }
}