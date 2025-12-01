using Incident.Models;

namespace Incident.Repositories;

public interface IIncidentRepository
{
    Task<IncidentRecord?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<IncidentRecord>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<int> GetTotalCountAsync(CancellationToken ct = default);
    Task<Guid> CreateAsync(IncidentRecord incident, CancellationToken ct = default);
    Task<bool> UpdateAsync(IncidentRecord incident, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}