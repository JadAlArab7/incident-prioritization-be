using Incident.Models;

namespace Incident.Repositories;

public interface IIncidentStatusRepository
{
    Task<IncidentStatusTransition?> GetTransitionAsync(Guid fromStatusId, string actionCode, CancellationToken ct = default);
    Task<List<IncidentStatusTransition>> GetTransitionsFromStatusAsync(Guid statusId, CancellationToken ct = default);
    Task<Guid?> GetStatusIdByCodeAsync(string code, CancellationToken ct = default);
    Task<string?> GetStatusCodeByIdAsync(Guid statusId, CancellationToken ct = default);
    Task InsertStatusHistoryAsync(IncidentStatusHistory history, CancellationToken ct = default);
    Task<List<IncidentStatusHistory>> GetStatusHistoryAsync(Guid incidentId, CancellationToken ct = default);
}