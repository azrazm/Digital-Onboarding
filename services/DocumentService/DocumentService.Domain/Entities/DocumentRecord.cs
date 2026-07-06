namespace DocumentService.Domain.Entities;

public class DocumentRecord
{
    public Guid DocumentId { get; set; }

    public string DocumentReference { get; set; } = string.Empty;

    public string OwnerReference { get; set; } = string.Empty;

    public string CustomerId { get; set; } = string.Empty;

    public string DocumentType { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public string Source { get; set; } = "DigitalOnboarding";

    public string UploadStatus { get; set; } = "PENDING";

    public DateTime UploadedAt { get; set; }

    public string? DmsActionId { get; set; } /*po uzoru*/

    public string? DmsUniqueObjectId { get; set; }
}