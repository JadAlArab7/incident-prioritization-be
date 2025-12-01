using Incident.Models;

namespace Incident.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUsernameAsync(string username);
    Task<IEnumerable<User>> GetByRoleAsync(string roleName);
    Task<User?> CreateAsync(User user);
    Task<bool> DeleteAsync(Guid id);
}