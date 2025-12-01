using System.ComponentModel.DataAnnotations;

namespace Incident.DTOs;

public class IncidentCreateRequestDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public Guid IncidentTypeId { get; set; }

    public Guid? LocationId { get; set; }
}