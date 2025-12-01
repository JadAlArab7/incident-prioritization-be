namespace Incident.DTOs;

public class LoginResponseDto
{
    public Guid UserId { get; set; }
    public string Jwt { get; set; } = string.Empty;
    public bool IsSec { get; set; }
    public bool IsSupervisor { get; set; }
    public bool IsOfficer { get; set; }
}