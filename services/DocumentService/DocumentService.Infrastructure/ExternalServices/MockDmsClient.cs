using DocumentService.Application.Interfaces;
using DocumentService.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace DocumentService.Infrastructure.ExternalServices;

public sealed class MockDmsClient : IDmsClient
{
    private readonly IConfiguration _configuration;

    public MockDmsClient(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<DocumentRecord> UploadAsync(
        DocumentRecord document,
        string base64Content)
    {
        var actionId =
            _configuration["Dms:ActionId"] ?? "010";

        var ownerReference =
            NormalizeReference(document.OwnerReference);

        var uniqueObjectId =
            $"DO-{ownerReference}";

        document.DmsActionId = actionId;
        document.DmsUniqueObjectId = uniqueObjectId;

        document.DocumentReference =
            $"{document.DocumentType}|{uniqueObjectId}|{document.DocumentId}";

        document.UploadStatus = "RECEIVED";

        return Task.FromResult(document);
    }

    private static string NormalizeReference(string value)
    {
        return value
            .Trim()
            .Replace("/", "-")
            .Replace("\\", "-")
            .Replace(" ", "-");
    }
}