using Incident.Models;

namespace Incident.Repositories;

public interface ILookupRepository
{
    Task<IEnumerable<User>> ListSecretariesAsync(CancellationToken ct = default);
    Task<IEnumerable<IncidentType>> ListIncidentTypesAsync(CancellationToken ct = default);
    Task<IEnumerable<IncidentStatus>> ListIncidentStatusesAsync(CancellationToken ct = default);
    Task<IEnumerable<Governorate>> ListGovernoratesAsync(CancellationToken ct = default);
    Task<IEnumerable<District>> ListDistrictsAsync(Guid governorateId, CancellationToken ct = default);
    Task<IEnumerable<Town>> ListTownsAsync(Guid districtId, CancellationToken ct = default);
    Task<IncidentStatus?> GetStatusByCodeAsync(string code, CancellationToken ct = default);
    Task<IncidentStatus?> GetStatusByIdAsync(Guid id, CancellationToken ct = default);
}