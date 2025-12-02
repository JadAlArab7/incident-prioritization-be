using System.Security.Claims;
using Incident.DTOs;
using Incident.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Incident.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponseDto<NotificationDto>>> GetNotifications(
        [FromQuery] bool unreadOnly = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var userId = GetCurrentUserId();
        var request = new PagedRequestDto { Page = page, PageSize = pageSize };
        var result = await _notificationService.GetUserNotificationsAsync(userId, unreadOnly, request, ct);
        return Ok(result);
    }

    [HttpGet("stats")]
    public async Task<ActionResult<NotificationStatsDto>> GetStats(CancellationToken ct = default)
    {
        var userId = GetCurrentUserId();
        var stats = await _notificationService.GetUserStatsAsync(userId, ct);
        return Ok(stats);
    }

    [HttpPost("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken ct = default)
    {
        var userId = GetCurrentUserId();
        var result = await _notificationService.MarkAsReadAsync(id, userId, ct);
        
        if (!result)
            return NotFound();
        
        return NoContent();
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken ct = default)
    {
        var userId = GetCurrentUserId();
        await _notificationService.MarkAllAsReadAsync(userId, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        var userId = GetCurrentUserId();
        var result = await _notificationService.DeleteNotificationAsync(id, userId, ct);
        
        if (!result)
            return NotFound();
        
        return NoContent();
    }

    [HttpPost("test")]
    public async Task<ActionResult<NotificationDto>> SendTestNotification(CancellationToken ct = default)
    {
        var userId = GetCurrentUserId();
        var notification = new CreateNotificationDto
        {
            UserId = userId,
            NotificationTypeCode = "system_notification",
            Title = "Test Notification",
            Message = "This is a test notification sent at " + DateTime.UtcNow.ToString("HH:mm:ss")
        };

        var result = await _notificationService.CreateAndSendNotificationAsync(notification, ct);
        return Ok(result);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid user token");
        return userId;
    }
}
