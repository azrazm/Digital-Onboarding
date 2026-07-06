using System.Text.Json;
using DocumentService.Application.Interfaces;
using DocumentService.Domain.Entities;

namespace DocumentService.Infrastructure.Repositories;

public sealed class JsonDocumentRepository : IDocumentRepository
{
    private readonly string _filePath;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public JsonDocumentRepository(string filePath)
    {
        _filePath = filePath;
    }

    public async Task CreateAsync(DocumentRecord document)
    {
        var documents = await ReadAllAsync();

        documents.Add(document);

        await SaveAllAsync(documents);
    }

    public async Task<DocumentRecord?> GetByIdAsync(Guid documentId)
    {
        var documents = await ReadAllAsync();

        return documents.FirstOrDefault(
            document => document.DocumentId == documentId);
    }

    private async Task<List<DocumentRecord>> ReadAllAsync()
    {
        if (!File.Exists(_filePath))
        {
            return new List<DocumentRecord>();
        }

        var fileInfo = new FileInfo(_filePath);

        if (fileInfo.Length == 0)
        {
            return new List<DocumentRecord>();
        }

        await using var stream = File.OpenRead(_filePath);

        var documents =
            await JsonSerializer.DeserializeAsync<List<DocumentRecord>>(
                stream,
                _jsonOptions);

        return documents ?? new List<DocumentRecord>();
    }

    private async Task SaveAllAsync(List<DocumentRecord> documents)
    {
        var directory = Path.GetDirectoryName(_filePath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(_filePath);

        await JsonSerializer.SerializeAsync(
            stream,
            documents,
            _jsonOptions);
    }
}