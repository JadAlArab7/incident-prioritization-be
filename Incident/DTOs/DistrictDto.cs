namespace Incident.DTOs;

public class DistrictDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public Guid GovernorateId { get; set; }
}