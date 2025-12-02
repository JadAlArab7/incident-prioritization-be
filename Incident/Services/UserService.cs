using Incident.DTOs;
using Incident.Infrastructure;
using Incident.Models;
using Incident.Repositories;

namespace Incident.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _userRepository.GetByIdAsync(id, ct);
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default)
    {
        return await _userRepository.GetAllAsync(ct);
    }

    public async Task<Guid> CreateAsync(CreateUserDto request, CancellationToken ct = default)
    {
        // Check if username already exists
        var existingUser = await _userRepository.GetByUsernameAsync(request.Username, ct);
        if (existingUser != null)
            throw new ArgumentException("Username already exists");

        // Get role
        var role = await _roleRepository.GetByNameAsync(request.Role, ct);
        if (role == null)
            throw new ArgumentException($"Role '{request.Role}' not found");

        // Hash password
        var (hash, salt) = PasswordHasher.HashPassword(request.Password);

        var user = new User
        {
            Username = request.Username,
            PasswordHash = hash,
            PasswordSalt = salt,
            RoleId = role.Id
        };

        return await _userRepository.CreateAsync(user, ct);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateUserDto request, CancellationToken ct = default)
    {
        // Check if user exists
        var existingUser = await _userRepository.GetByIdAsync(id, ct);
        if (existingUser == null)
            return false;

        // Check if username is being changed and if it already exists
        if (existingUser.Username != request.Username)
        {
            var userWithSameUsername = await _userRepository.GetByUsernameAsync(request.Username, ct);
            if (userWithSameUsername != null)
                throw new ArgumentException("Username already exists");
        }

        // Get role
        var role = await _roleRepository.GetByNameAsync(request.Role, ct);
        if (role == null)
            throw new ArgumentException($"Role '{request.Role}' not found");

        // Update user fields
        existingUser.Username = request.Username;
        existingUser.RoleId = role.Id;

        // Update password only if provided
        if (!string.IsNullOrEmpty(request.Password))
        {
            var (hash, salt) = PasswordHasher.HashPassword(request.Password);
            existingUser.PasswordHash = hash;
            existingUser.PasswordSalt = salt;
        }

        return await _userRepository.UpdateAsync(existingUser, ct);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        return await _userRepository.DeleteAsync(id, ct);
    }
}