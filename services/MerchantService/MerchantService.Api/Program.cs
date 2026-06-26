var builder = WebApplication.CreateBuilder(args);

// Omogućava korištenje controller-based endpointa.
builder.Services.AddControllers();

// Generiše OpenAPI dokument iz controller ruta.
builder.Services.AddOpenApi();

var app = builder.Build();

// Swagger dokumentacija je dostupna samo u Development okruženju.
if (app.Environment.IsDevelopment())
{
    // OpenAPI JSON bit će dostupan na:
    // /openapi/v1.json
    app.MapOpenApi();

    // Swagger UI bit će dostupan na:
    // /swagger
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