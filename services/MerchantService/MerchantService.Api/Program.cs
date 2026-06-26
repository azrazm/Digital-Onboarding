using MerchantService.Application.Interfaces;
using MerchantService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
var merchantsJsonPath = Path.GetFullPath(
    Path.Combine(
        builder.Environment.ContentRootPath,
        "..",
        "MerchantService.Infrastructure",
        "MockData",
        "merchants.json"));

builder.Services.AddScoped<IMerchantRepository>(
    _ => new JsonMerchantRepository(merchantsJsonPath));

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "Merchant Service API v1"
        );
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();