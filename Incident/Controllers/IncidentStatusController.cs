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

    public IncidentStatusController(IIncidentStatusService statusService)
    {
        _statusService = statusService;
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] IncidentStatusUpdateRequestDto request,
        CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(new { error = "User not authenticated" });
        }

        var result = await _statusService.UpdateStatusAsync(
            id,
            userId.Value,
            request.Action,
            request.Comment,
            request.NewSentToUserId,
            ct);

        if (!result.Success)
        {
            return result.StatusCode switch
            {
                400 => BadRequest(new { error = result.ErrorMessage }),
                403 => StatusCode(403, new { error = result.ErrorMessage }),
                404 => NotFound(new { error = result.ErrorMessage }),
                409 => Conflict(new { error = result.ErrorMessage }),
                _ => StatusCode(500, new { error = result.ErrorMessage })
            };
        }

        return Ok(result.Data);
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