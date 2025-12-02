using Incident.DTOs;

namespace Incident.Services;

public interface INotificationService
{
    Task<NotificationDto> CreateAndSendNotificationAsync(CreateNotificationDto request, CancellationToken ct = default);
    Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId, CancellationToken ct = default);
    Task<bool> MarkAllAsReadAsync(Guid userId, CancellationToken ct = default);
    Task<PagedResponseDto<NotificationDto>> GetUserNotificationsAsync(
        Guid userId, 
        bool unreadOnly, 
        PagedRequestDto request, 
        CancellationToken ct = default);
    Task<NotificationStatsDto> GetUserStatsAsync(Guid userId, CancellationToken ct = default);
    Task<bool> DeleteNotificationAsync(Guid id, Guid userId, CancellationToken ct = default);
}
