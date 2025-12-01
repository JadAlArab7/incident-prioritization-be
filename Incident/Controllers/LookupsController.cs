using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Incident.DTOs;
using Incident.Services;

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
    public async Task<ActionResult<IEnumerable<IncidentTypeDto>>> GetIncidentTypes()
    {
        var incidentTypes = await _lookupService.GetIncidentTypesAsync();
        var dtos = incidentTypes.Select(it => new IncidentTypeDto
        {
            Id = it.Id,
            Name = it.Name,
            Description = it.Description
        });

        return Ok(dtos);
    }

    [HttpGet("incident-statuses")]
    public async Task<ActionResult<IEnumerable<IncidentStatusDto>>> GetIncidentStatuses()
    {
        var statuses = await _lookupService.GetIncidentStatusesAsync();
        var dtos = statuses.Select(s => new IncidentStatusDto
        {
            Id = s.Id,
            Code = s.Code,
            Name = s.Name,
            Description = s.Description,
            IsTerminal = s.IsTerminal
        });

        return Ok(dtos);
    }

    [HttpGet("governorates")]
    public async Task<ActionResult<IEnumerable<GovernorateDto>>> GetGovernorates()
    {
        var governorates = await _lookupService.GetGovernoratesAsync();
        var dtos = governorates.Select(g => new GovernorateDto
        {
            Id = g.Id,
            Name = g.Name,
            NameAr = g.NameAr
        });

        return Ok(dtos);
    }

    [HttpGet("districts")]
    public async Task<ActionResult<IEnumerable<DistrictDto>>> GetDistricts()
    {
        var districts = await _lookupService.GetDistrictsAsync();
        var dtos = districts.Select(d => new DistrictDto
        {
            Id = d.Id,
            Name = d.Name,
            NameAr = d.NameAr,
            GovernorateId = d.GovernorateId
        });

        return Ok(dtos);
    }

    [HttpGet("districts/by-governorate/{governorateId:guid}")]
    public async Task<ActionResult<IEnumerable<DistrictDto>>> GetDistrictsByGovernorate(Guid governorateId)
    {
        var districts = await _lookupService.GetDistrictsByGovernorateAsync(governorateId);
        var dtos = districts.Select(d => new DistrictDto
        {
            Id = d.Id,
            Name = d.Name,
            NameAr = d.NameAr,
            GovernorateId = d.GovernorateId
        });

        return Ok(dtos);
    }

    [HttpGet("towns")]
    public async Task<ActionResult<IEnumerable<TownDto>>> GetTowns()
    {
        var towns = await _lookupService.GetTownsAsync();
        var dtos = towns.Select(t => new TownDto
        {
            Id = t.Id,
            Name = t.Name,
            NameAr = t.NameAr,
            DistrictId = t.DistrictId
        });

        return Ok(dtos);
    }

    [HttpGet("towns/by-district/{districtId:guid}")]
    public async Task<ActionResult<IEnumerable<TownDto>>> GetTownsByDistrict(Guid districtId)
    {
        var towns = await _lookupService.GetTownsByDistrictAsync(districtId);
        var dtos = towns.Select(t => new TownDto
        {
            Id = t.Id,
            Name = t.Name,
            NameAr = t.NameAr,
            DistrictId = t.DistrictId
        });

        return Ok(dtos);
    }

    [HttpGet("roles")]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
    {
        var roles = await _lookupService.GetRolesAsync();
        var dtos = roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name
        });

        return Ok(dtos);
    }
}