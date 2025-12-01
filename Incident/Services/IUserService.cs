using Incident.DTOs;
using Incident.Models;

namespace Incident.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(Guid id);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName);
    Task<User?> CreateUserAsync(CreateUserDto createUserDto);
    Task<bool> DeleteUserAsync(Guid id);
    Task<bool> ValidateUserCredentialsAsync(string username, string password);
}