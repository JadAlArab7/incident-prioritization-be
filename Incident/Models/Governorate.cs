namespace Incident.Models;

public class Governorate
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}