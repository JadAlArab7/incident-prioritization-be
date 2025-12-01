using Incident.DTOs;
using Incident.Models;

namespace Incident.Services;

public interface IUserService
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<User>> GetAllAsync(CancellationToken ct = default);
    Task<Guid> CreateAsync(CreateUserDto request, CancellationToken ct = default);
}