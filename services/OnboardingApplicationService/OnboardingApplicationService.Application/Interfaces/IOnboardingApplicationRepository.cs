using OnboardingApplicationService.Domain.Entities;

namespace OnboardingApplicationService.Application.Interfaces;

public interface IOnboardingApplicationRepository
{
    /*ova ruta je vezana za internu logiku*/
    Task<IReadOnlyList<OnboardingApplication>> GetAllAsync();

    Task CreateAsync(OnboardingApplication application);

    Task<OnboardingApplication?> GetByIdAsync(Guid applicationId);

    Task<int> CountAsync();
}