namespace Incident.Models;

public class Notification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid NotificationTypeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? RelatedEntityType { get; set; }
    public Guid? RelatedEntityId { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Metadata { get; set; }
    
    // Navigation properties
    public User? User { get; set; }
    public NotificationType? NotificationType { get; set; }
}

public class NotificationType
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Well-known notification type codes
    public static readonly string IncidentAssigned = "incident_assigned";
    public static readonly string IncidentStatusChanged = "incident_status_changed";
    public static readonly string IncidentCreated = "incident_created";
    public static readonly string SystemNotification = "system_notification";
}
