using Incident.DTOs;
using Incident.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Incident.Controllers;

[ApiController]
[Route("api/incidents")]
[Authorize]
public class IncidentStatusController : ControllerBase
{
    private readonly IIncidentStatusService _statusService;
    private readonly ILogger<IncidentStatusController> _logger;

    public IncidentStatusController(
        IIncidentStatusService statusService,
        ILogger<IncidentStatusController> logger)
    {
        _statusService = statusService;
        _logger = logger;
    }

    /// <summary>
    /// Update incident status using action-based workflow
    /// </summary>
    /// <param name="id">Incident ID</param>
    /// <param name="request">Status update request with action code</param>
    /// <returns>Updated incident with action flags</returns>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(IncidentStatusUpdateResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateStatus(
        [FromRoute] Guid id,
        [FromBody] IncidentStatusUpdateRequestDto request,
        CancellationToken ct)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null)
        {
            return Unauthorized("User ID not found in token");
        }

        var result = await _statusService.UpdateStatusAsync(
            id,
            currentUserId.Value,
            request.Action,
            request.Comment,
            request.NewSentToUserId,
            ct);

        if (!result.Success)
        {
            return result.ErrorCode switch
            {
                400 => BadRequest(new { error = result.ErrorMessage }),
                403 => Forbid(),
                404 => NotFound(new { error = result.ErrorMessage }),
                409 => Conflict(new { error = result.ErrorMessage }),
                _ => BadRequest(new { error = result.ErrorMessage })
            };
        }

        return Ok(result.Response);
    }

    /// <summary>
    /// Get incident by ID with computed action flags for current user
    /// </summary>
    /// <param name="id">Incident ID</param>
    /// <returns>Incident details with action flags</returns>
    [HttpGet("{id}/details")]
    [ProducesResponseType(typeof(IncidentDetailResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetIncidentWithFlags(
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null)
        {
            return Unauthorized("User ID not found in token");
        }

        var result = await _statusService.GetIncidentWithFlagsAsync(id, currentUserId.Value, ct);
        if (result == null)
        {
            return NotFound(new { error = $"Incident with ID {id} not found" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Get action flags for an incident for the current user
    /// </summary>
    /// <param name="id">Incident ID</param>
    /// <returns>Action flags</returns>
    [HttpGet("{id}/actions")]
    [ProducesResponseType(typeof(IncidentActionFlags), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetActionFlags(
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null)
        {
            return Unauthorized("User ID not found in token");
        }

        var flags = await _statusService.ComputeActionFlagsAsync(id, currentUserId.Value, ct);
        return Ok(flags);
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