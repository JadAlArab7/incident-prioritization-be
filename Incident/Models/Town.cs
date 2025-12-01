namespace Incident.Models;

public class Town
{
    public Guid Id { get; set; }
    public Guid DistrictId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public double? Lat { get; set; }
    public double? Lng { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation
    public District? District { get; set; }
}