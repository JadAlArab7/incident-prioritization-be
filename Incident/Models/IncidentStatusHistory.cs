namespace Incident.Models;

public class IncidentStatusHistory
{
    public Guid Id { get; set; }
    public Guid IncidentId { get; set; }
    public Guid FromStatusId { get; set; }
    public Guid ToStatusId { get; set; }
    public Guid ChangedByUserId { get; set; }
    public string? Comment { get; set; }
    public DateTime ChangedAt { get; set; }
}