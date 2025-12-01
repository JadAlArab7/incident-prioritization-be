using System.ComponentModel.DataAnnotations;

namespace Incident.DTOs;

public class IncidentStatusUpdateRequestDto
{
    [Required]
    public string Action { get; set; } = string.Empty;

    public string? Comment { get; set; }

    public Guid? NewSentToUserId { get; set; }
}