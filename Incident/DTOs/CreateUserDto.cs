using System.ComponentModel.DataAnnotations;

namespace Incident.DTOs;

public class CreateUserDto
{
    [Required]
    [MinLength(3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string RoleName { get; set; } = string.Empty;
}