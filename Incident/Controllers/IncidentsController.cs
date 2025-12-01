using System.Security.Claims;
using Incident.DTOs;
using Incident.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Incident.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IncidentsController : ControllerBase
{
    private readonly IIncidentService _incidentService;

    public IncidentsController(IIncidentService incidentService)
    {
        _incidentService = incidentService;
    }

    [HttpPost]
    public async Task<ActionResult<IncidentResponseDto>> Create(
        [FromBody] IncidentCreateRequestDto request,
        CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        var result = await _incidentService.CreateAsync(request, userId, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<IncidentResponseDto>> Update(
        Guid id,
        [FromBody] IncidentUpdateRequestDto request,
        CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        var result = await _incidentService.UpdateAsync(id, request, userId, ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        var userRole = GetCurrentUserRole();
        var deleted = await _incidentService.DeleteAsync(id, userId, userRole, ct);
        
        if (!deleted)
            return NotFound();
            
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<IncidentResponseDto>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _incidentService.GetByIdAsync(id, ct);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponseDto<IncidentResponseDto>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var userId = GetCurrentUserId();
        var request = new PagedRequestDto { Page = page, PageSize = pageSize };
        var result = await _incidentService.ListForUserAsync(userId, request, ct);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<IncidentResponseDto>> UpdateStatus(
        Guid id,
        [FromBody] UpdateStatusRequestDto request,
        CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        var result = await _incidentService.UpdateStatusAsync(id, request.StatusId, userId, ct);
        return Ok(result);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid user token");
        return userId;
    }

    private string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }
}