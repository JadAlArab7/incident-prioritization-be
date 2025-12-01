using Incident.Models;

namespace Incident.Repositories;

public interface IIncidentStatusRepository
{
    Task<IncidentStatus?> GetByIdAsync(Guid id);
    Task<IncidentStatus?> GetByCodeAsync(string code);
    Task<IEnumerable<IncidentStatus>> GetAllAsync();
    Task<IEnumerable<IncidentStatusTransition>> GetTransitionsFromStatusAsync(Guid fromStatusId);
    Task<IncidentStatusTransition?> GetTransitionAsync(Guid fromStatusId, string actionCode);
    Task AddStatusHistoryAsync(IncidentStatusHistory history);
    Task<IEnumerable<IncidentStatusHistory>> GetStatusHistoryAsync(Guid incidentId);
}