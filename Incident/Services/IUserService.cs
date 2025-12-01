using Incident.DTOs;
using Incident.Models;

namespace Incident.Services;

public interface IUserService
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUsernameAsync(string username);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> CreateAsync(CreateUserDto createUserDto);
    Task<User?> UpdateAsync(Guid id, CreateUserDto updateUserDto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}