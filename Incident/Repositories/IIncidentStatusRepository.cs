using Incident.Models;

namespace Incident.Repositories;

public interface IIncidentStatusRepository
{
    Task<IncidentStatus?> GetByIdAsync(Guid id);
    Task<IncidentStatus?> GetByCodeAsync(string code);
    Task<IEnumerable<IncidentStatusTransition>> GetAllowedTransitionsAsync(
        Guid fromStatusId, 
        string action, 
        Guid userId, 
        Guid incidentCreatorId, 
        Guid? incidentAssignedOfficerId);
}