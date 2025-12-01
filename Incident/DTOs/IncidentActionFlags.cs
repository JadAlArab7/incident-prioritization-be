namespace Incident.DTOs;

public class IncidentActionFlags
{
    public List<string> NextActions { get; set; } = new();
    public bool CanSendToReview { get; set; }
    public bool CanAccept { get; set; }
    public bool CanReject { get; set; }
    public bool CanEdit { get; set; }
}