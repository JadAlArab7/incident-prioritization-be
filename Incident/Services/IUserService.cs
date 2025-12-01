using Incident.DTOs;
using Incident.Models;

namespace Incident.Services;

public interface IUserService
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUsernameAsync(string username);
    Task<IEnumerable<User>> GetAllAsync();
    Task<IEnumerable<User>> GetOfficersAsync();
    Task<User> CreateAsync(CreateUserDto dto);
    Task<bool> DeleteAsync(Guid id);
}