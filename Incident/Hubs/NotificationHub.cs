using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Incident.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        if (userId.HasValue)
        {
            // Add user to their personal group
            await Groups.AddToGroupAsync(Context.ConnectionId, GetUserGroupName(userId.Value));
            _logger.LogInformation("User {UserId} connected to NotificationHub with connection {ConnectionId}", 
                userId.Value, Context.ConnectionId);
        }
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        if (userId.HasValue)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetUserGroupName(userId.Value));
            _logger.LogInformation("User {UserId} disconnected from NotificationHub", userId.Value);
        }
        
        await base.OnDisconnectedAsync(exception);
    }

    private Guid? GetUserId()
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        return null;
    }

    public static string GetUserGroupName(Guid userId) => $"user_{userId}";
}
