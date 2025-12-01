using Incident.Models;

namespace Incident.Repositories;

public interface IIncidentRepository
{
    Task<Guid> CreateAsync(IncidentRecord entity, IEnumerable<Guid> typeIds, CancellationToken ct = default);
    Task<bool> UpdateAsync(IncidentRecord entity, IEnumerable<Guid> typeIds, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<IncidentRecord?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(IEnumerable<IncidentRecord> Items, int TotalCount)> ListForUserAsync(
        Guid userId, 
        int page, 
        int pageSize, 
        CancellationToken ct = default);
    Task<Guid> CreateLocationAsync(Location location, CancellationToken ct = default);
    Task<bool> UpdateLocationAsync(Location location, CancellationToken ct = default);
    Task<bool> UpdateStatusAsync(Guid incidentId, Guid statusId, CancellationToken ct = default);
}