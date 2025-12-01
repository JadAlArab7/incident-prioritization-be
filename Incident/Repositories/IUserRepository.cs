using Incident.Models;

namespace Incident.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByIdAsync(Guid id);
    Task<Guid> CreateUserAsync(string username, byte[] passwordHash, byte[] passwordSalt, Guid roleId);
    Task<bool> UserExistsAsync(string username);
}