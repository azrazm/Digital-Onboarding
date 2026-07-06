using DocumentService.Domain.Entities;

namespace DocumentService.Application.Interfaces;

public interface IDocumentRepository
{
    Task CreateAsync(DocumentRecord document);

    Task<DocumentRecord?> GetByIdAsync(Guid documentId);
}