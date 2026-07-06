using System.ComponentModel.DataAnnotations;

namespace DocumentService.Api.Models.Requests;

public class UploadDocumentRequest
{
    [Required]
    public string OwnerReference { get; set; } = string.Empty;  /*trenutno, posto nemam applicationId i customerId, bilo kakva referenca */

    [Required]
    public string CustomerId { get; set; } = string.Empty;

    [Required]
    public string DocumentType { get; set; } = string.Empty;

    [Required]
    public string FileName { get; set; } = string.Empty;

    [Required]
    public string ContentType { get; set; } = string.Empty;

    [Required]
    public string Base64Content { get; set; } = string.Empty;

    public string Source { get; set; } = "DigitalOnboarding";
}