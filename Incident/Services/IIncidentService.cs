using Incident.DTOs;

namespace Incident.Services;

public interface IIncidentService
{
    Task<IncidentResponseDto> CreateAsync(IncidentCreateRequestDto request, Guid currentUserId, CancellationToken ct = default);
    Task<IncidentResponseDto> UpdateAsync(Guid id, IncidentUpdateRequestDto request, Guid currentUserId, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, Guid currentUserId, string userRole, CancellationToken ct = default);
    Task<IncidentResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResponseDto<IncidentResponseDto>> ListForUserAsync(Guid userId, bool includeAssigned, PagedRequestDto request, CancellationToken ct = default);
}