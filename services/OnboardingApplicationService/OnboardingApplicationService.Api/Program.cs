using OnboardingApplicationService.Application.Interfaces;
using OnboardingApplicationService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var onboardingApplicationsPath = Path.GetFullPath(
    Path.Combine(
        builder.Environment.ContentRootPath,
        "..",
        "OnboardingApplicationService.Infrastructure",
        "MockData",
        "onboarding-applications.json"));

builder.Services.AddScoped<IOnboardingApplicationRepository>(
    _ => new JsonOnboardingApplicationRepository(
        onboardingApplicationsPath));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "Onboarding Application Service API v1");
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();