namespace Incident.Models;

public class District
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public Guid GovernorateId { get; set; }
}