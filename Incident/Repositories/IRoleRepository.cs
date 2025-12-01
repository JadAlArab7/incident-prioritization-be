using Incident.Models;

namespace Incident.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Role?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<IEnumerable<Role>> GetAllAsync(CancellationToken ct = default);
}