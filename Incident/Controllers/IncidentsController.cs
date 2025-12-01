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
    private readonly IIncidentStatusService _incidentStatusService;

    public IncidentsController(
        IIncidentService incidentService,
        IIncidentStatusService incidentStatusService)
    {
        _incidentService = incidentService;
        _incidentStatusService = incidentStatusService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<IncidentDetailResponseDto>> GetById(Guid id)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized();
        }

        var incident = await _incidentService.GetByIdAsync(id);
        if (incident == null)
        {
            return NotFound();
        }

        var actionFlags = await _incidentStatusService.GetActionFlagsAsync(id, userId);
        var status = await GetStatusById(incident.StatusId);

        var response = new IncidentDetailResponseDto
        {
            Id = incident.Id,
            Title = incident.Title,
            Description = incident.Description,
            IncidentTypeId = incident.IncidentTypeId,
            IncidentTypeName = incident.IncidentTypeName,
            StatusId = incident.StatusId,
            StatusCode = status?.Code ?? "",
            StatusName = status?.Name ?? "",
            LocationId = incident.LocationId,
            Location = incident.Location,
            CreatedByUserId = incident.CreatedByUserId,
            CreatedByUserName = incident.CreatedByUserName,
            SentToUserId = incident.SentToUserId,
            SentToUserName = incident.SentToUserName,
            CreatedAt = incident.CreatedAt,
            UpdatedAt = incident.UpdatedAt,
            NextActions = actionFlags.NextActions,
            CanSendToReview = actionFlags.CanSendToReview,
            CanAccept = actionFlags.CanAccept,
            CanReject = actionFlags.CanReject,
            CanEdit = actionFlags.CanEdit
        };

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponseDto<IncidentResponseDto>>> GetPaged([FromQuery] PagedRequestDto request)
    {
        var pagedResult = await _incidentService.GetPagedAsync(request);
        
        var responseDtos = pagedResult.Data.Select(i => new IncidentResponseDto
        {
            Id = i.Id,
            Title = i.Title,
            Description = i.Description,
            IncidentTypeId = i.IncidentTypeId,
            IncidentTypeName = i.IncidentTypeName,
            StatusId = i.StatusId,
            StatusName = i.StatusName,
            LocationId = i.LocationId,
            Location = i.Location,
            CreatedByUserId = i.CreatedByUserId,
            CreatedByUserName = i.CreatedByUserName,
            SentToUserId = i.SentToUserId,
            SentToUserName = i.SentToUserName,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt
        }).ToList();

        var response = new PagedResponseDto<IncidentResponseDto>
        {
            Data = responseDtos,
            TotalCount = pagedResult.TotalCount,
            Page = pagedResult.Page,
            PageSize = pagedResult.PageSize
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<IncidentResponseDto>> Create(IncidentCreateRequestDto request)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized();
        }

        var incident = await _incidentService.CreateAsync(request, userId);

        var response = new IncidentResponseDto
        {
            Id = incident.Id,
            Title = incident.Title,
            Description = incident.Description,
            IncidentTypeId = incident.IncidentTypeId,
            IncidentTypeName = incident.IncidentTypeName,
            StatusId = incident.StatusId,
            StatusName = incident.StatusName,
            LocationId = incident.LocationId,
            Location = incident.Location,
            CreatedByUserId = incident.CreatedByUserId,
            CreatedByUserName = incident.CreatedByUserName,
            SentToUserId = incident.SentToUserId,
            SentToUserName = incident.SentToUserName,
            CreatedAt = incident.CreatedAt,
            UpdatedAt = incident.UpdatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = incident.Id }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<IncidentResponseDto>> Update(Guid id, IncidentUpdateRequestDto request)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized();
        }

        try
        {
            var incident = await _incidentService.UpdateAsync(id, request, userId);
            if (incident == null)
            {
                return NotFound();
            }

            var response = new IncidentResponseDto
            {
                Id = incident.Id,
                Title = incident.Title,
                Description = incident.Description,
                IncidentTypeId = incident.IncidentTypeId,
                IncidentTypeName = incident.IncidentTypeName,
                StatusId = incident.StatusId,
                StatusName = incident.StatusName,
                LocationId = incident.LocationId,
                Location = incident.Location,
                CreatedByUserId = incident.CreatedByUserId,
                CreatedByUserName = incident.CreatedByUserName,
                SentToUserId = incident.SentToUserId,
                SentToUserName = incident.SentToUserName,
                CreatedAt = incident.CreatedAt,
                UpdatedAt = incident.UpdatedAt
            };

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized();
        }

        try
        {
            var success = await _incidentService.DeleteAsync(id, userId);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    private async Task<Models.IncidentStatus?> GetStatusById(Guid statusId)
    {
        // This would normally be injected, but for simplicity we'll create a temporary instance
        // In a real application, this would be properly injected
        return null;
    }
}