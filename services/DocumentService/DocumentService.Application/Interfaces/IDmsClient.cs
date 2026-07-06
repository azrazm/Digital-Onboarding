using DocumentService.Domain.Entities;

namespace DocumentService.Application.Interfaces;

public interface IDmsClient
{
    Task<DocumentRecord> UploadAsync(
        DocumentRecord document,
        string base64Content);
}