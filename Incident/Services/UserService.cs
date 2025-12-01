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

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _userRepository.GetByUsernameAsync(username);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<User> CreateAsync(CreateUserDto createUserDto)
    {
        // Check if username or email already exists
        if (await _userRepository.UsernameExistsAsync(createUserDto.Username))
        {
            throw new InvalidOperationException("Username already exists");
        }

        if (await _userRepository.EmailExistsAsync(createUserDto.Email))
        {
            throw new InvalidOperationException("Email already exists");
        }

        // Get role
        var role = await _roleRepository.GetByCodeAsync(createUserDto.RoleCode);
        if (role == null)
        {
            throw new InvalidOperationException($"Role '{createUserDto.RoleCode}' not found");
        }

        var user = new User
        {
            Username = createUserDto.Username,
            Email = createUserDto.Email,
            FullName = createUserDto.FullName,
            RoleId = role.Id,
            PasswordHash = _passwordHasher.HashPassword(createUserDto.Password)
        };

        return await _userRepository.CreateAsync(user);
    }

    public async Task<User?> UpdateAsync(Guid id, CreateUserDto updateUserDto)
    {
        var existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser == null)
        {
            return null;
        }

        // Check if username or email already exists (for other users)
        if (await _userRepository.UsernameExistsAsync(updateUserDto.Username) && 
            existingUser.Username != updateUserDto.Username)
        {
            throw new InvalidOperationException("Username already exists");
        }

        if (await _userRepository.EmailExistsAsync(updateUserDto.Email) && 
            existingUser.Email != updateUserDto.Email)
        {
            throw new InvalidOperationException("Email already exists");
        }

        // Get role
        var role = await _roleRepository.GetByCodeAsync(updateUserDto.RoleCode);
        if (role == null)
        {
            throw new InvalidOperationException($"Role '{updateUserDto.RoleCode}' not found");
        }

        existingUser.Username = updateUserDto.Username;
        existingUser.Email = updateUserDto.Email;
        existingUser.FullName = updateUserDto.FullName;
        existingUser.RoleId = role.Id;

        // Only update password if provided
        if (!string.IsNullOrEmpty(updateUserDto.Password))
        {
            existingUser.PasswordHash = _passwordHasher.HashPassword(updateUserDto.Password);
        }

        return await _userRepository.UpdateAsync(existingUser);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _userRepository.DeleteAsync(id);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _userRepository.ExistsAsync(id);
    }
}