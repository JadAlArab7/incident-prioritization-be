using Incident.DTOs;

namespace Incident.Services;

public interface IIncidentService
{
    Task<IncidentResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResponseDto<IncidentResponseDto>> GetAllAsync(PagedRequestDto request, CancellationToken ct = default);
    Task<IncidentResponseDto> CreateAsync(IncidentCreateRequestDto request, Guid createdByUserId, CancellationToken ct = default);
    Task<IncidentResponseDto?> UpdateAsync(Guid id, IncidentUpdateRequestDto request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}