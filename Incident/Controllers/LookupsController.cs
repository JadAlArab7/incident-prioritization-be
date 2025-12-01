using Incident.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Incident.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LookupsController : ControllerBase
{
    private readonly ILookupService _lookupService;

    public LookupsController(ILookupService lookupService)
    {
        _lookupService = lookupService;
    }

    [HttpGet("governorates")]
    public async Task<IActionResult> GetGovernorates(CancellationToken ct)
    {
        var governorates = await _lookupService.GetGovernoratesAsync(ct);
        return Ok(governorates);
    }

    [HttpGet("governorates/{governorateId:guid}/districts")]
    public async Task<IActionResult> GetDistricts(Guid governorateId, CancellationToken ct)
    {
        var districts = await _lookupService.GetDistrictsByGovernorateAsync(governorateId, ct);
        return Ok(districts);
    }

    [HttpGet("districts/{districtId:guid}/towns")]
    public async Task<IActionResult> GetTowns(Guid districtId, CancellationToken ct)
    {
        var towns = await _lookupService.GetTownsByDistrictAsync(districtId, ct);
        return Ok(towns);
    }

    [HttpGet("incident-types")]
    public async Task<IActionResult> GetIncidentTypes(CancellationToken ct)
    {
        var types = await _lookupService.GetIncidentTypesAsync(ct);
        return Ok(types);
    }

    [HttpGet("incident-statuses")]
    public async Task<IActionResult> GetIncidentStatuses(CancellationToken ct)
    {
        var statuses = await _lookupService.GetIncidentStatusesAsync(ct);
        return Ok(statuses);
    }
}