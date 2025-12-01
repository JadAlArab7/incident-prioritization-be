using Incident.Models;

namespace Incident.Repositories;

public interface ILookupRepository
{
    Task<IEnumerable<IncidentType>> GetIncidentTypesAsync();
    Task<IEnumerable<IncidentStatus>> GetIncidentStatusesAsync();
    Task<IEnumerable<Governorate>> GetGovernoratesAsync();
    Task<IEnumerable<District>> GetDistrictsAsync();
    Task<IEnumerable<District>> GetDistrictsByGovernorateAsync(Guid governorateId);
    Task<IEnumerable<Town>> GetTownsAsync();
    Task<IEnumerable<Town>> GetTownsByDistrictAsync(Guid districtId);
    Task<IEnumerable<Role>> GetRolesAsync();
}