using Incident.Models;
using Incident.DTOs;

namespace Incident.Repositories;

public interface IIncidentRepository
{
    Task<IncidentRecord?> GetByIdAsync(Guid id);
    Task<PagedResponseDto<IncidentRecord>> GetPagedAsync(PagedRequestDto request);
    Task<IncidentRecord> CreateAsync(IncidentRecord incident);
    Task<IncidentRecord?> UpdateAsync(IncidentRecord incident);
    Task<bool> UpdateStatusAsync(Guid incidentId, Guid statusId, Guid? sentToUserId);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}