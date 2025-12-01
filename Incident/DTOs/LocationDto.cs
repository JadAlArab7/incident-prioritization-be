namespace Incident.DTOs;

public class LocationDto
{
    public Guid Id { get; set; }
    public Guid? GovernorateId { get; set; }
    public string? GovernorateName { get; set; }
    public Guid? DistrictId { get; set; }
    public string? DistrictName { get; set; }
    public Guid? TownId { get; set; }
    public string? TownName { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}