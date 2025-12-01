namespace Incident.Models;

public class IncidentRecord
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? SentToUserId { get; set; }
    public Guid CreatedByUserId { get; set; }
    public Guid? LocationId { get; set; }
    public string? Priority { get; set; }
    public string? SuggestedActionsTaken { get; set; }
    public Guid StatusId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation
    public User? SentToUser { get; set; }
    public User? CreatedByUser { get; set; }
    public Location? Location { get; set; }
    public IncidentStatus? Status { get; set; }
    public List<IncidentType> Types { get; set; } = new();
}