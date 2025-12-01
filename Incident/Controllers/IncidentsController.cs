using Incident.DTOs;
using Incident.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Incident.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IncidentsController : ControllerBase
{
    private readonly IIncidentService _incidentService;
    private readonly IIncidentStatusService _statusService;

    public IncidentsController(
        IIncidentService incidentService,
        IIncidentStatusService statusService)
    {
        _incidentService = incidentService;
        _statusService = statusService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequestDto request, CancellationToken ct)
    {
        var result = await _incidentService.GetAllAsync(request, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var incident = await _incidentService.GetByIdAsync(id, ct);

        if (incident == null)
        {
            return NotFound(new { error = $"Incident with ID {id} not found" });
        }

        return Ok(incident);
    }

    [HttpGet("{id:guid}/details")]
    public async Task<IActionResult> GetDetails(Guid id, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(new { error = "User not authenticated" });
        }

        var result = await _statusService.GetIncidentWithFlagsAsync(id, userId.Value, ct);

        if (result == null)
        {
            return NotFound(new { error = $"Incident with ID {id} not found" });
        }

        return Ok(result);
    }

    [HttpGet("{id:guid}/actions")]
    public async Task<IActionResult> GetActions(Guid id, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(new { error = "User not authenticated" });
        }

        var flags = await _statusService.ComputeActionFlagsAsync(id, userId.Value, ct);
        return Ok(flags);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] IncidentCreateRequestDto request, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(new { error = "User not authenticated" });
        }

        var incident = await _incidentService.CreateAsync(request, userId.Value, ct);
        return CreatedAtAction(nameof(GetById), new { id = incident.Id }, incident);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] IncidentUpdateRequestDto request, CancellationToken ct)
    {
        var incident = await _incidentService.UpdateAsync(id, request, ct);

        if (incident == null)
        {
            return NotFound(new { error = $"Incident with ID {id} not found" });
        }

        return Ok(incident);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _incidentService.DeleteAsync(id, ct);

        if (!deleted)
        {
            return NotFound(new { error = $"Incident with ID {id} not found" });
        }

        return NoContent();
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return null;
        }
        return userId;
    }
}