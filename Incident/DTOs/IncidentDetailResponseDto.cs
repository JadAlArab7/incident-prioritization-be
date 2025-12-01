namespace Incident.DTOs;

public sealed class IncidentDetailResponseDto
{
    // Incident fields
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid IncidentTypeId { get; set; }
    public string? IncidentTypeName { get; set; }
    public Guid StatusId { get; set; }
    public string? StatusCode { get; set; }
    public string? StatusName { get; set; }
    public Guid? LocationId { get; set; }
    public LocationDto? Location { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public Guid? SentToUserId { get; set; }
    public string? SentToUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Action flags
    public List<string> NextActions { get; set; } = new();
    public bool CanSendToReview { get; set; }
    public bool CanAccept { get; set; }
    public bool CanReject { get; set; }
    public bool CanEdit { get; set; }
}