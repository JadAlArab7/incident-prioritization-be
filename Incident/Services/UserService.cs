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

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _userRepository.GetByUsernameAsync(username);
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName)
    {
        return await _userRepository.GetByRoleAsync(roleName);
    }

    public async Task<User?> CreateUserAsync(CreateUserDto createUserDto)
    {
        var (hash, salt) = PasswordHasher.HashPassword(createUserDto.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = createUserDto.Username,
            PasswordHash = hash,
            PasswordSalt = salt,
            RoleId = createUserDto.RoleId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdUser = await _userRepository.CreateAsync(user);
        if (createdUser != null)
        {
            // Fetch the user with role information
            return await _userRepository.GetByIdAsync(createdUser.Id);
        }

        return null;
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        return await _userRepository.DeleteAsync(id);
    }

    public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
        {
            return false;
        }

        return PasswordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
    }
}