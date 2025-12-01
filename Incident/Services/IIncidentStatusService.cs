using Incident.DTOs;

namespace Incident.Services;

public interface IIncidentStatusService
{
    Task<StatusUpdateResult> UpdateStatusAsync(
        Guid incidentId,
        Guid currentUserId,
        string actionCode,
        string? comment,
        Guid? newSentToUserId,
        CancellationToken ct = default);

    Task<IncidentActionFlags> ComputeActionFlagsAsync(
        Guid incidentId,
        Guid currentUserId,
        CancellationToken ct = default);

    Task<IncidentDetailResponseDto?> GetIncidentWithFlagsAsync(
        Guid incidentId,
        Guid currentUserId,
        CancellationToken ct = default);
}