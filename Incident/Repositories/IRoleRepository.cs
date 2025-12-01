using Incident.Models;

namespace Incident.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Role?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<List<Role>> GetAllAsync(CancellationToken ct = default);
}