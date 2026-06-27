using OnboardingApplicationService.Domain.Entities;

namespace OnboardingApplicationService.Application.Interfaces;

public interface IOnboardingApplicationRepository
{
    Task CreateAsync(OnboardingApplication application);

    Task<OnboardingApplication?> GetByIdAsync(Guid applicationId);

    Task<int> CountAsync();
}