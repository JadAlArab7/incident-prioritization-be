using Incident.DTOs;
using Incident.Repositories;

namespace Incident.Services;

public class LookupService : ILookupService
{
    private readonly ILookupRepository _lookupRepository;

    public LookupService(ILookupRepository lookupRepository)
    {
        _lookupRepository = lookupRepository;
    }

    public async Task<IEnumerable<UserSummaryDto>> ListSecretariesAsync(CancellationToken ct = default)
    {
        var users = await _lookupRepository.ListSecretariesAsync(ct);
        return users.Select(u => new UserSummaryDto
        {
            Id = u.Id,
            Username = u.Username,
            RoleName = u.Role?.Name
        });
    }

    public async Task<IEnumerable<IncidentTypeDto>> ListIncidentTypesAsync(CancellationToken ct = default)
    {
        var types = await _lookupRepository.ListIncidentTypesAsync(ct);
        return types.Select(t => new IncidentTypeDto
        {
            Id = t.Id,
            Name = t.Name,
            NameEn = t.NameEn,
            NameAr = t.NameAr
        });
    }

    public async Task<IEnumerable<IncidentStatusDto>> ListIncidentStatusesAsync(CancellationToken ct = default)
    {
        var statuses = await _lookupRepository.ListIncidentStatusesAsync(ct);
        return statuses.Select(s => new IncidentStatusDto
        {
            Id = s.Id,
            Code = s.Code,
            Name = s.Name,
            NameAr = s.NameAr
        });
    }

    public async Task<IEnumerable<GeoLookupDto>> ListGovernoratesAsync(CancellationToken ct = default)
    {
        var governorates = await _lookupRepository.ListGovernoratesAsync(ct);
        return governorates.Select(g => new GeoLookupDto
        {
            Id = g.Id,
            Name = g.Name,
            NameAr = g.NameAr
        });
    }

    public async Task<IEnumerable<GeoLookupDto>> ListDistrictsAsync(Guid governorateId, CancellationToken ct = default)
    {
        var districts = await _lookupRepository.ListDistrictsAsync(governorateId, ct);
        return districts.Select(d => new GeoLookupDto
        {
            Id = d.Id,
            Name = d.Name,
            NameAr = d.NameAr
        });
    }

    public async Task<IEnumerable<TownDto>> ListTownsAsync(Guid districtId, CancellationToken ct = default)
    {
        var towns = await _lookupRepository.ListTownsAsync(districtId, ct);
        return towns.Select(t => new TownDto
        {
            Id = t.Id,
            Name = t.Name,
            NameAr = t.NameAr,
            Lat = t.Lat,
            Lng = t.Lng
        });
    }
}