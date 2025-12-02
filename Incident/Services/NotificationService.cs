using Incident.DTOs;
using Incident.Hubs;
using Incident.Models;
using Incident.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace Incident.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        INotificationRepository notificationRepository,
        IHubContext<NotificationHub> hubContext,
        ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task<NotificationDto> CreateAndSendNotificationAsync(CreateNotificationDto request, CancellationToken ct = default)
    {
        // Get notification type
        var notificationType = await _notificationRepository.GetNotificationTypeByCodeAsync(request.NotificationTypeCode, ct);
        if (notificationType == null)
            throw new ArgumentException($"Invalid notification type code: {request.NotificationTypeCode}");

        // Create notification
        var notification = new Notification
        {
            UserId = request.UserId,
            NotificationTypeId = notificationType.Id,
            Title = request.Title,
            Message = request.Message,
            RelatedEntityType = request.RelatedEntityType,
            RelatedEntityId = request.RelatedEntityId,
            IsRead = false
        };

        var notificationId = await _notificationRepository.CreateAsync(notification, ct);

        // Get the created notification with full details
        var createdNotification = await _notificationRepository.GetByIdAsync(notificationId, ct);
        if (createdNotification == null)
            throw new InvalidOperationException("Failed to retrieve created notification");

        var notificationDto = MapToDto(createdNotification);

        // Send real-time notification via SignalR
        await SendNotificationToUserAsync(request.UserId, notificationDto);

        _logger.LogInformation("Notification {NotificationId} created and sent to user {UserId}", 
            notificationId, request.UserId);

        return notificationDto;
    }

    public async Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId, CancellationToken ct = default)
    {
        var result = await _notificationRepository.MarkAsReadAsync(notificationId, userId, ct);
        
        if (result)
        {
            // Send real-time update to user
            await _hubContext.Clients
                .Group(NotificationHub.GetUserGroupName(userId))
                .SendAsync("NotificationRead", new { NotificationId = notificationId }, ct);

            // Also send updated unread count
            var unreadCount = await _notificationRepository.GetUnreadCountAsync(userId, ct);
            await _hubContext.Clients
                .Group(NotificationHub.GetUserGroupName(userId))
                .SendAsync("UnreadCountUpdated", new { UnreadCount = unreadCount }, ct);

            _logger.LogInformation("Notification {NotificationId} marked as read for user {UserId}", 
                notificationId, userId);
        }

        return result;
    }

    public async Task<bool> MarkAllAsReadAsync(Guid userId, CancellationToken ct = default)
    {
        var result = await _notificationRepository.MarkAllAsReadAsync(userId, ct);
        
        if (result)
        {
            // Send real-time update to user
            await _hubContext.Clients
                .Group(NotificationHub.GetUserGroupName(userId))
                .SendAsync("AllNotificationsRead", new { }, ct);

            await _hubContext.Clients
                .Group(NotificationHub.GetUserGroupName(userId))
                .SendAsync("UnreadCountUpdated", new { UnreadCount = 0 }, ct);

            _logger.LogInformation("All notifications marked as read for user {UserId}", userId);
        }

        return result;
    }

    public async Task<PagedResponseDto<NotificationDto>> GetUserNotificationsAsync(
        Guid userId, bool unreadOnly, PagedRequestDto request, CancellationToken ct = default)
    {
        var (items, totalCount) = await _notificationRepository.GetUserNotificationsAsync(
            userId, unreadOnly, request.Page, request.PageSize, ct);

        return new PagedResponseDto<NotificationDto>
        {
            Items = items.Select(MapToDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<NotificationStatsDto> GetUserStatsAsync(Guid userId, CancellationToken ct = default)
    {
        var (allNotifications, totalCount) = await _notificationRepository.GetUserNotificationsAsync(
            userId, false, 1, 1, default);
        
        var unreadCount = await _notificationRepository.GetUnreadCountAsync(userId, default);

        return new NotificationStatsDto
        {
            TotalCount = totalCount,
            UnreadCount = unreadCount
        };
    }

    public async Task<bool> DeleteNotificationAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        var result = await _notificationRepository.DeleteAsync(id, userId, ct);
        
        if (result)
        {
            // Notify user about deletion
            await _hubContext.Clients
                .Group(NotificationHub.GetUserGroupName(userId))
                .SendAsync("NotificationDeleted", new { NotificationId = id }, ct);

            // Update unread count
            var unreadCount = await _notificationRepository.GetUnreadCountAsync(userId, ct);
            await _hubContext.Clients
                .Group(NotificationHub.GetUserGroupName(userId))
                .SendAsync("UnreadCountUpdated", new { UnreadCount = unreadCount }, ct);
        }

        return result;
    }

    private async Task SendNotificationToUserAsync(Guid userId, NotificationDto notification)
    {
        try
        {
            // Send to user's personal group
            await _hubContext.Clients
                .Group(NotificationHub.GetUserGroupName(userId))
                .SendAsync("ReceiveNotification", notification);

            // Also send updated unread count
            var unreadCount = await _notificationRepository.GetUnreadCountAsync(userId, default);
            await _hubContext.Clients
                .Group(NotificationHub.GetUserGroupName(userId))
                .SendAsync("UnreadCountUpdated", new { UnreadCount = unreadCount });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification via SignalR to user {UserId}", userId);
        }
    }

    private static NotificationDto MapToDto(Notification notification)
    {
        return new NotificationDto
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Title = notification.Title,
            Message = notification.Message,
            TypeCode = notification.NotificationType?.Code ?? string.Empty,
            TypeName = notification.NotificationType?.Name ?? string.Empty,
            RelatedEntityType = notification.RelatedEntityType,
            RelatedEntityId = notification.RelatedEntityId,
            IsRead = notification.IsRead,
            ReadAt = notification.ReadAt,
            CreatedAt = notification.CreatedAt
        };
    }
}
