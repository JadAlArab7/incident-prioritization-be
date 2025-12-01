namespace Incident.DTOs;

public class LocationDto
{
    public double Lat { get; set; }
    public double Lng { get; set; }
    public Guid? GovernorateId { get; set; }
    public Guid? DistrictId { get; set; }
    public Guid? TownId { get; set; }
    public string? AddressText { get; set; }
}