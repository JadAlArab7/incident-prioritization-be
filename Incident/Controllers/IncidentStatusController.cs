using Incident.DTOs;
using Incident.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Incident.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IncidentStatusController : ControllerBase
{
    private readonly IIncidentStatusService _incidentStatusService;

    public IncidentStatusController(IIncidentStatusService incidentStatusService)
    {
        _incidentStatusService = incidentStatusService;
    }

    [HttpGet("{incidentId:guid}/actions")]
    public async Task<ActionResult<IncidentActionFlags>> GetActions(Guid incidentId)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized();
        }

        var actionFlags = await _incidentStatusService.GetActionFlagsAsync(incidentId, userId);
        return Ok(actionFlags);
    }

    [HttpPatch("{incidentId:guid}/status")]
    public async Task<ActionResult<IncidentStatusUpdateResponseDto>> UpdateStatus(
        Guid incidentId,
        IncidentStatusUpdateRequestDto request)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized();
        }

        var result = await _incidentStatusService.UpdateStatusAsync(incidentId, userId, request);
        if (!result.Success)
        {
            return BadRequest(new { error = result.ErrorMessage });
        }

        var response = new IncidentStatusUpdateResponseDto
        {
            Incident = new IncidentResponseDto
            {
                Id = result.Incident.Id,
                Title = result.Incident.Title,
                Description = result.Incident.Description,
                IncidentTypeId = result.Incident.IncidentTypeId,
                IncidentTypeName = result.Incident.IncidentTypeName,
                StatusId = result.Incident.StatusId,
                StatusName = result.Incident.StatusName,
                LocationId = result.Incident.LocationId,
                Location = result.Incident.Location,
                CreatedByUserId = result.Incident.CreatedByUserId,
                CreatedByUserName = result.Incident.CreatedByUserName,
                SentToUserId = result.Incident.SentToUserId,
                SentToUserName = result.Incident.SentToUserName,
                CreatedAt = result.Incident.CreatedAt,
                UpdatedAt = result.Incident.UpdatedAt
            },
            NextActions = result.ActionFlags.NextActions,
            CanSendToReview = result.ActionFlags.CanSendToReview,
            CanAccept = result.ActionFlags.CanAccept,
            CanReject = result.ActionFlags.CanReject,
            CanEdit = result.ActionFlags.CanEdit
        };

        return Ok(response);
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}