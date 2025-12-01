using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Incident.DTOs;
using Incident.Infrastructure;
using Incident.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace Incident.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUserRepository userRepository, JwtSettings jwtSettings)
    {
        _userRepository = userRepository;
        _jwtSettings = jwtSettings;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username, ct);
        if (user == null)
            return null;

        if (!PasswordHasher.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            return null;

        var token = GenerateJwtToken(user.Id, user.Username, user.Role?.Name ?? "");

        return new LoginResponseDto
        {
            Token = token,
            UserId = user.Id,
            Username = user.Username,
            Role = user.Role?.Name ?? ""
        };
    }

    private string GenerateJwtToken(Guid userId, string username, string role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}