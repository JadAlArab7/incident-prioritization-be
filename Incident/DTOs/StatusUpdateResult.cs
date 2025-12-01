using Incident.Models;

namespace Incident.DTOs;

public class StatusUpdateResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public IncidentRecord? Incident { get; set; }
    public IncidentActionFlags? ActionFlags { get; set; }
}