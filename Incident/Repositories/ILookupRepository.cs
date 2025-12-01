using Incident.Models;

namespace Incident.Repositories;

public interface ILookupRepository
{
    Task<IEnumerable<Governorate>> GetGovernoratesAsync();
    Task<IEnumerable<District>> GetDistrictsAsync(Guid governorateId);
    Task<IEnumerable<Town>> GetTownsAsync(Guid districtId);
    Task<IEnumerable<IncidentType>> GetIncidentTypesAsync();
    Task<IEnumerable<IncidentStatus>> GetIncidentStatusesAsync();
    Task<IEnumerable<User>> GetOfficersAsync();
}