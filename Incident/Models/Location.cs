namespace Incident.Models;

public class Location
{
    public Guid Id { get; set; }
    public double Lat { get; set; }
    public double Lng { get; set; }
    public Guid? GovernorateId { get; set; }
    public Guid? DistrictId { get; set; }
    public Guid? TownId { get; set; }
    public string? AddressText { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation
    public Governorate? Governorate { get; set; }
    public District? District { get; set; }
    public Town? Town { get; set; }
}