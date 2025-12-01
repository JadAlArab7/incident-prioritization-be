using Incident.Models;
using Incident.Repositories;

namespace Incident.Services;

public class LookupService : ILookupService
{
    private readonly ILookupRepository _lookupRepository;

    public LookupService(ILookupRepository lookupRepository)
    {
        _lookupRepository = lookupRepository;
    }

    public async Task<List<Governorate>> GetGovernoratesAsync(CancellationToken ct = default)
    {
        return await _lookupRepository.GetGovernoratesAsync(ct);
    }

    public async Task<List<District>> GetDistrictsByGovernorateAsync(Guid governorateId, CancellationToken ct = default)
    {
        return await _lookupRepository.GetDistrictsByGovernorateAsync(governorateId, ct);
    }

    public async Task<List<Town>> GetTownsByDistrictAsync(Guid districtId, CancellationToken ct = default)
    {
        return await _lookupRepository.GetTownsByDistrictAsync(districtId, ct);
    }

    public async Task<List<IncidentType>> GetIncidentTypesAsync(CancellationToken ct = default)
    {
        return await _lookupRepository.GetIncidentTypesAsync(ct);
    }

    public async Task<List<IncidentStatus>> GetIncidentStatusesAsync(CancellationToken ct = default)
    {
        return await _lookupRepository.GetIncidentStatusesAsync(ct);
    }
}