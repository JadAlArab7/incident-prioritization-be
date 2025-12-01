namespace Incident.Models;

public class IncidentStatusTransition
{
    public Guid Id { get; set; }
    public Guid FromStatusId { get; set; }
    public Guid ToStatusId { get; set; }
    public string ActionCode { get; set; } = string.Empty;
    public string Initiator { get; set; } = string.Empty; // 'creator' or 'officer'
    public bool IsActive { get; set; } = true;
}