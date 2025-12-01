using Incident.DTOs;
using Incident.Infrastructure;
using Incident.Models;
using Incident.Repositories;

namespace Incident.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _userRepository.GetByIdAsync(id, ct);
    }

    public async Task<List<User>> GetAllAsync(CancellationToken ct = default)
    {
        return await _userRepository.GetAllAsync(ct);
    }

    public async Task<Guid> CreateAsync(CreateUserDto request, CancellationToken ct = default)
    {
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            FullName = request.FullName,
            RoleId = request.RoleId,
            IsActive = true
        };

        return await _userRepository.CreateAsync(user, ct);
    }
}