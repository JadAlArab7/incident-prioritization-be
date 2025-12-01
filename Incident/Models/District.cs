namespace Incident.Models;

public class District
{
    public Guid Id { get; set; }
    public Guid GovernorateId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    // Navigation
    public Governorate? Governorate { get; set; }
}