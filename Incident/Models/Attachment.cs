namespace Incident.Models;

public class Attachment
{
    public Guid Id { get; set; }
    public Guid IncidentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public string? StoragePath { get; set; }
    public DateTime UploadedAt { get; set; }
}