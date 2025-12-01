namespace Incident.Models;

public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    // Well-known role names
    public static readonly string Secretary = "secretary";
    public static readonly string Officer = "officer";
    public static readonly string Supervisor = "supervisor";
}