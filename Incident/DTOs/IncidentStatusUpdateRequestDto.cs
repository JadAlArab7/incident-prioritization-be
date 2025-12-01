using System.ComponentModel.DataAnnotations;

namespace Incident.DTOs;

public sealed class IncidentStatusUpdateRequestDto
{
    [Required]
    [RegularExpression("^(send_to_review|accept|reject)$", ErrorMessage = "Action must be 'send_to_review', 'accept', or 'reject'")]
    public string Action { get; set; } = string.Empty;
    
    public string? Comment { get; set; }
    
    public Guid? NewSentToUserId { get; set; }
}