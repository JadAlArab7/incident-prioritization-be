using Incident.DTOs;
using Incident.Models;

namespace Incident.Services;

public interface IIncidentService
{
    Task<IncidentRecord?> GetByIdAsync(Guid id);
    Task<PagedResponseDto<IncidentRecord>> GetPagedAsync(PagedRequestDto request);
    Task<IncidentRecord> CreateAsync(IncidentCreateRequestDto request, Guid userId);
    Task<IncidentRecord?> UpdateAsync(Guid id, IncidentUpdateRequestDto request, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
}