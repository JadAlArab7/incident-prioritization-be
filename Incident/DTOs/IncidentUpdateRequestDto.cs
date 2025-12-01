namespace Incident.DTOs;

public class IncidentUpdateRequestDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Guid? IncidentTypeId { get; set; }
    public Guid? LocationId { get; set; }
}