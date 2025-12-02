using Incident.Models;

namespace Incident.Repositories;

public interface INotificationRepository
{
    Task<Guid> CreateAsync(Notification notification, CancellationToken ct = default);
    Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId, CancellationToken ct = default);
    Task<bool> MarkAllAsReadAsync(Guid userId, CancellationToken ct = default);
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(IEnumerable<Notification> Items, int TotalCount)> GetUserNotificationsAsync(
        Guid userId, 
        bool unreadOnly, 
        int page, 
        int pageSize, 
        CancellationToken ct = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default);
    Task<NotificationType?> GetNotificationTypeByCodeAsync(string code, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken ct = default);
}
