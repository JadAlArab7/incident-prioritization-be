using Incident.DTOs;

namespace Incident.Services;

public interface ILookupService
{
    Task<IEnumerable<UserSummaryDto>> ListSecretariesAsync(CancellationToken ct = default);
    Task<IEnumerable<IncidentTypeDto>> ListIncidentTypesAsync(CancellationToken ct = default);
    Task<IEnumerable<IncidentStatusDto>> ListIncidentStatusesAsync(CancellationToken ct = default);
    Task<IEnumerable<GeoLookupDto>> ListGovernoratesAsync(CancellationToken ct = default);
    Task<IEnumerable<GeoLookupDto>> ListDistrictsAsync(Guid governorateId, CancellationToken ct = default);
    Task<IEnumerable<TownDto>> ListTownsAsync(Guid districtId, CancellationToken ct = default);
}

public class TownDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public double? Lat { get; set; }
    public double? Lng { get; set; }
}