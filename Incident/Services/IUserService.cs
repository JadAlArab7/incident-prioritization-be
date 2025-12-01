using Incident.DTOs;
using Incident.Models;

namespace Incident.Services;

public interface IUserService
{
    Task<User?> GetByIdAsync(Guid id);
    Task<Guid> CreateUserAsync(CreateUserDto request);
    Task<bool> UserExistsAsync(string username);
}