using Incident.Models;

namespace Incident.Services;

public interface ILookupService
{
    Task<List<Governorate>> GetGovernoratesAsync(CancellationToken ct = default);
    Task<List<District>> GetDistrictsByGovernorateAsync(Guid governorateId, CancellationToken ct = default);
    Task<List<Town>> GetTownsByDistrictAsync(Guid districtId, CancellationToken ct = default);
    Task<List<IncidentType>> GetIncidentTypesAsync(CancellationToken ct = default);
    Task<List<IncidentStatus>> GetIncidentStatusesAsync(CancellationToken ct = default);
}