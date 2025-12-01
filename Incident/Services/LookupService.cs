using Incident.DTOs;
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

    public async Task<IEnumerable<Governorate>> GetGovernoratesAsync()
    {
        return await _lookupRepository.GetGovernoratesAsync();
    }

    public async Task<IEnumerable<District>> GetDistrictsAsync(Guid governorateId)
    {
        return await _lookupRepository.GetDistrictsAsync(governorateId);
    }

    public async Task<IEnumerable<Town>> GetTownsAsync(Guid districtId)
    {
        return await _lookupRepository.GetTownsAsync(districtId);
    }

    public async Task<IEnumerable<IncidentType>> GetIncidentTypesAsync()
    {
        return await _lookupRepository.GetIncidentTypesAsync();
    }

    public async Task<IEnumerable<IncidentStatus>> GetIncidentStatusesAsync()
    {
        return await _lookupRepository.GetIncidentStatusesAsync();
    }

    public async Task<IEnumerable<User>> GetOfficersAsync()
    {
        return await _lookupRepository.GetOfficersAsync();
    }
}