using Incident.DTOs;

namespace Incident.Services;

public interface IIncidentService
{
    Task<IncidentResponseDto> CreateAsync(IncidentCreateRequestDto request, Guid currentUserId, CancellationToken ct = default);
    Task<IncidentResponseDto> UpdateAsync(Guid id, IncidentUpdateRequestDto request, Guid currentUserId, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, Guid currentUserId, string userRole, CancellationToken ct = default);
    Task<IncidentResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResponseDto<IncidentResponseDto>> ListForUserAsync(Guid userId, PagedRequestDto request, CancellationToken ct = default);
    Task<IncidentResponseDto> UpdateStatusAsync(Guid id, Guid statusId, Guid currentUserId, CancellationToken ct = default);
}