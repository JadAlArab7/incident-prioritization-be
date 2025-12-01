using Incident.DTOs;

namespace Incident.Services;

public interface IIncidentStatusService
{
    Task<IncidentActionFlags> GetActionFlagsAsync(Guid incidentId, Guid userId);
    Task<StatusUpdateResult> UpdateStatusAsync(Guid incidentId, Guid userId, IncidentStatusUpdateRequestDto request);
}