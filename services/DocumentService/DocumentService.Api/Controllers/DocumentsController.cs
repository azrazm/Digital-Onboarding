using DocumentService.Api.Models.Requests;
using DocumentService.Application.Interfaces;
using DocumentService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DocumentService.Api.Controllers;

[ApiController]
[Route("api/v1/documents")]
public class DocumentsController : ControllerBase
{
    private const int MaxFileSizeBytes = 10 * 1024 * 1024;

    private static readonly HashSet<string> AllowedContentTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "application/pdf",
            "image/png",
            "image/jpeg"
        };

    private readonly IDocumentRepository _documentRepository;
    private readonly IDmsClient _dmsClient;

    public DocumentsController(
        IDocumentRepository documentRepository,
        IDmsClient dmsClient)
    {
        _documentRepository = documentRepository;
        _dmsClient = dmsClient;
    }

    [HttpPost("upload")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadDocument(
        [FromBody] UploadDocumentRequest request)
    {
        var contentType = request.ContentType.Trim();

        if (!AllowedContentTypes.Contains(contentType))
        {
            return BadRequest(new
            {
                status = 400,
                message = "ContentType mora biti application/pdf, image/png ili image/jpeg.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        if (!TryDecodeBase64(request.Base64Content, out var decodedBytes))
        {
            return BadRequest(new
            {
                status = 400,
                message = "Base64Content nije validan base64 string.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        if (decodedBytes.Length > MaxFileSizeBytes)
        {
            return BadRequest(new
            {
                status = 400,
                message = "Dokument ne smije biti veći od 10 MB.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        var document = new DocumentRecord
        {
            DocumentId = Guid.NewGuid(),
            OwnerReference = request.OwnerReference.Trim(),
            CustomerId = request.CustomerId.Trim(),
            DocumentType = request.DocumentType.Trim().ToUpperInvariant(),
            FileName = request.FileName.Trim(),
            ContentType = contentType,
            Source = string.IsNullOrWhiteSpace(request.Source)
                ? "DigitalOnboarding"
                : request.Source.Trim(),
            UploadStatus = "PENDING",
            UploadedAt = DateTime.UtcNow
        };

        var uploadedDocument =
            await _dmsClient.UploadAsync(
                document,
                request.Base64Content);

        await _documentRepository.CreateAsync(uploadedDocument);

        return CreatedAtAction(
            nameof(GetDocumentById),
            new { documentId = uploadedDocument.DocumentId },
            MapDocumentDetails(uploadedDocument));
    }

    [HttpGet("{documentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDocumentById(string documentId)
    {
        if (!Guid.TryParse(documentId, out var parsedDocumentId))
        {
            return BadRequest(new
            {
                status = 400,
                message = "Document ID nije u ispravnom UUID formatu.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        var document =
            await _documentRepository.GetByIdAsync(parsedDocumentId);

        if (document is null)
        {
            return NotFound(new
            {
                status = 404,
                message = "Dokument nije pronađen.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        return Ok(MapDocumentDetails(document));
    }

    private static bool TryDecodeBase64(
        string base64Content,
        out byte[] decodedBytes)
    {
        try
        {
            decodedBytes = Convert.FromBase64String(base64Content);
            return true;
        }
        catch (FormatException)
        {
            decodedBytes = Array.Empty<byte>();
            return false;
        }
    }

    private static object MapDocumentDetails(DocumentRecord document)
    {
        return new
        {
            documentId = document.DocumentId,
            documentReference = document.DocumentReference,
            ownerReference = document.OwnerReference,
            customerId = document.CustomerId,
            documentType = document.DocumentType,
            fileName = document.FileName,
            contentType = document.ContentType,
            source = document.Source,
            uploadStatus = document.UploadStatus,
            uploadedAt = document.UploadedAt,
            dmsActionId = document.DmsActionId,
            dmsUniqueObjectId = document.DmsUniqueObjectId
        };
    }
}