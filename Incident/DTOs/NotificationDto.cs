namespace Incident.DTOs;

public class NotificationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string TypeCode { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public string? RelatedEntityType { get; set; }
    public Guid? RelatedEntityId { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateNotificationDto
{
    public Guid UserId { get; set; }
    public string NotificationTypeCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? RelatedEntityType { get; set; }
    public Guid? RelatedEntityId { get; set; }
}

public class MarkNotificationReadDto
{
    public Guid NotificationId { get; set; }
}

public class NotificationStatsDto
{
    public int TotalCount { get; set; }
    public int UnreadCount { get; set; }
}
