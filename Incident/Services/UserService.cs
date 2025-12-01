using Incident.DTOs;
using Incident.Infrastructure;
using Incident.Models;
using Incident.Repositories;

namespace Incident.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<Guid> CreateUserAsync(CreateUserDto request)
    {
        var role = await _roleRepository.GetByNameAsync(request.RoleName);
        
        if (role == null)
        {
            throw new ArgumentException($"Role '{request.RoleName}' not found.");
        }

        var (hash, salt) = _passwordHasher.HashPassword(request.Password);

        return await _userRepository.CreateUserAsync(request.Username, hash, salt, role.Id);
    }

    public async Task<bool> UserExistsAsync(string username)
    {
        return await _userRepository.UserExistsAsync(username);
    }
}