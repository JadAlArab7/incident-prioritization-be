using Incident.DTOs;
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

    [HttpGet("incident-types")]
    public async Task<ActionResult<IEnumerable<IncidentTypeDto>>> GetIncidentTypes(CancellationToken ct)
    {
        var result = await _lookupService.ListIncidentTypesAsync(ct);
        return Ok(result);
    }

    [HttpGet("incident-statuses")]
    public async Task<ActionResult<IEnumerable<IncidentStatusDto>>> GetIncidentStatuses(CancellationToken ct)
    {
        var result = await _lookupService.ListIncidentStatusesAsync(ct);
        return Ok(result);
    }

    [HttpGet("governorates")]
    public async Task<ActionResult<IEnumerable<GeoLookupDto>>> GetGovernorates(CancellationToken ct)
    {
        var result = await _lookupService.ListGovernoratesAsync(ct);
        return Ok(result);
    }

    [HttpGet("districts")]
    public async Task<ActionResult<IEnumerable<GeoLookupDto>>> GetDistricts(
        [FromQuery] Guid governorateId,
        CancellationToken ct)
    {
        var result = await _lookupService.ListDistrictsAsync(governorateId, ct);
        return Ok(result);
    }

    [HttpGet("towns")]
    public async Task<ActionResult<IEnumerable<TownDto>>> GetTowns(
        [FromQuery] Guid districtId,
        CancellationToken ct)
    {
        var result = await _lookupService.ListTownsAsync(districtId, ct);
        return Ok(result);
    }

    [HttpGet("officers")]
    public async Task<ActionResult<IEnumerable<UserSummaryDto>>> GetOfficers(CancellationToken ct)
    {
        var result = await _lookupService.ListSecretariesAsync(ct);
        return Ok(result);
    }
}