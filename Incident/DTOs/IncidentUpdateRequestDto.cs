namespace Incident.DTOs;

public class IncidentUpdateRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? SentToUserId { get; set; }
    public LocationDto? Location { get; set; }
    public List<Guid> TypeIds { get; set; } = new();
    public string? Priority { get; set; }
    public string? SuggestedActionsTaken { get; set; }
    public Guid? StatusId { get; set; }
}