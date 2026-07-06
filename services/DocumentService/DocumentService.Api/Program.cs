using DocumentService.Application.Interfaces;
using DocumentService.Infrastructure.ExternalServices;
using DocumentService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var documentsJsonPath = Path.GetFullPath(
    Path.Combine(
        builder.Environment.ContentRootPath,
        "..",
        "DocumentService.Infrastructure",
        "MockData",
        "documents.json"));

builder.Services.AddScoped<IDocumentRepository>(
    _ => new JsonDocumentRepository(documentsJsonPath));

builder.Services.AddScoped<IDmsClient, MockDmsClient>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "Document Service API v1");
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();