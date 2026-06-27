using ProductService.Application.Interfaces;
using ProductService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var productConfigurationPath = Path.GetFullPath(
    Path.Combine(
        builder.Environment.ContentRootPath,
        "..",
        "ProductService.Infrastructure",
        "MockData",
        "product-configuration.json"));

builder.Services.AddScoped<IProductConfigurationRepository>(
    _ => new JsonProductConfigurationRepository(
        productConfigurationPath));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "Product Service API v1");
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();