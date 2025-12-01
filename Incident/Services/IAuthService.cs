using Incident.DTOs;

namespace Incident.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> AuthenticateAsync(LoginRequestDto request);
}