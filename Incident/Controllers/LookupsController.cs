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
    public async Task<ActionResult> GetGovernorates()
    {
        var governorates = await _lookupService.GetGovernoratesAsync();
        return Ok(governorates);
    }

    [HttpGet("districts/{governorateId:guid}")]
    public async Task<ActionResult> GetDistricts(Guid governorateId)
    {
        var districts = await _lookupService.GetDistrictsAsync(governorateId);
        return Ok(districts);
    }

    [HttpGet("towns/{districtId:guid}")]
    public async Task<ActionResult> GetTowns(Guid districtId)
    {
        var towns = await _lookupService.GetTownsAsync(districtId);
        return Ok(towns);
    }

    [HttpGet("incident-types")]
    public async Task<ActionResult> GetIncidentTypes()
    {
        var types = await _lookupService.GetIncidentTypesAsync();
        return Ok(types);
    }

    [HttpGet("incident-statuses")]
    public async Task<ActionResult> GetIncidentStatuses()
    {
        var statuses = await _lookupService.GetIncidentStatusesAsync();
        return Ok(statuses);
    }

    [HttpGet("officers")]
    public async Task<ActionResult> GetOfficers()
    {
        var officers = await _lookupService.GetOfficersAsync();
        var officerDtos = officers.Select(o => new
        {
            Id = o.Id,
            FullName = o.FullName
        });

        return Ok(officerDtos);
    }
}