using Incident.Infrastructure;
using Incident.Models;
using Npgsql;

namespace Incident.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly IDbHelper _db;

    public NotificationRepository(IDbHelper db)
    {
        _db = db;
    }

    public async Task<Guid> CreateAsync(Notification notification, CancellationToken ct = default)
    {
        const string sql = @"
            INSERT INTO incident.notifications 
                (id, user_id, notification_type_id, title, message, 
                 related_entity_type, related_entity_id, is_read, metadata)
            VALUES 
                (@id, @userId, @notificationTypeId, @title, @message,
                 @relatedEntityType, @relatedEntityId, @isRead, @metadata::jsonb)
            RETURNING id";

        var id = Guid.NewGuid();
        await _db.ExecuteScalarAsync<Guid>(sql, ct,
            new NpgsqlParameter("@id", id),
            new NpgsqlParameter("@userId", notification.UserId),
            new NpgsqlParameter("@notificationTypeId", notification.NotificationTypeId),
            new NpgsqlParameter("@title", notification.Title),
            new NpgsqlParameter("@message", notification.Message),
            new NpgsqlParameter("@relatedEntityType", (object?)notification.RelatedEntityType ?? DBNull.Value),
            new NpgsqlParameter("@relatedEntityId", (object?)notification.RelatedEntityId ?? DBNull.Value),
            new NpgsqlParameter("@isRead", notification.IsRead),
            new NpgsqlParameter("@metadata", (object?)notification.Metadata ?? DBNull.Value));

        return id;
    }

    public async Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId, CancellationToken ct = default)
    {
        const string sql = @"
            UPDATE incident.notifications
            SET is_read = TRUE, read_at = NOW()
            WHERE id = @notificationId AND user_id = @userId AND is_read = FALSE";

        var rows = await _db.ExecuteNonQueryAsync(sql, ct,
            new NpgsqlParameter("@notificationId", notificationId),
            new NpgsqlParameter("@userId", userId));

        return rows > 0;
    }

    public async Task<bool> MarkAllAsReadAsync(Guid userId, CancellationToken ct = default)
    {
        const string sql = @"
            UPDATE incident.notifications
            SET is_read = TRUE, read_at = NOW()
            WHERE user_id = @userId AND is_read = FALSE";

        var rows = await _db.ExecuteNonQueryAsync(sql, ct,
            new NpgsqlParameter("@userId", userId));

        return rows > 0;
    }

    public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT 
                n.id, n.user_id, n.notification_type_id, n.title, n.message,
                n.related_entity_type, n.related_entity_id, n.is_read, n.read_at,
                n.created_at, n.metadata,
                nt.code as type_code, nt.name as type_name, nt.name_ar as type_name_ar
            FROM incident.notifications n
            JOIN incident.notification_types nt ON n.notification_type_id = nt.id
            WHERE n.id = @id";

        await using var reader = await _db.ExecuteReaderAsync(sql, ct, new NpgsqlParameter("@id", id));

        if (!await reader.ReadAsync(ct))
            return null;

        return MapNotificationFromReader(reader);
    }

    public async Task<(IEnumerable<Notification> Items, int TotalCount)> GetUserNotificationsAsync(
        Guid userId, bool unreadOnly, int page, int pageSize, CancellationToken ct = default)
    {
        var whereClause = unreadOnly 
            ? "WHERE n.user_id = @userId AND n.is_read = FALSE"
            : "WHERE n.user_id = @userId";

        var countSql = $@"
            SELECT COUNT(*) 
            FROM incident.notifications n 
            {whereClause}";

        var totalCount = await _db.ExecuteScalarAsync<long>(countSql, ct, 
            new NpgsqlParameter("@userId", userId));

        var sql = $@"
            SELECT 
                n.id, n.user_id, n.notification_type_id, n.title, n.message,
                n.related_entity_type, n.related_entity_id, n.is_read, n.read_at,
                n.created_at, n.metadata,
                nt.code as type_code, nt.name as type_name, nt.name_ar as type_name_ar
            FROM incident.notifications n
            JOIN incident.notification_types nt ON n.notification_type_id = nt.id
            {whereClause}
            ORDER BY n.created_at DESC
            LIMIT @pageSize OFFSET @offset";

        var notifications = new List<Notification>();
        await using var reader = await _db.ExecuteReaderAsync(sql, ct,
            new NpgsqlParameter("@userId", userId),
            new NpgsqlParameter("@pageSize", pageSize),
            new NpgsqlParameter("@offset", (page - 1) * pageSize));

        while (await reader.ReadAsync(ct))
        {
            notifications.Add(MapNotificationFromReader(reader));
        }

        return (notifications, (int)totalCount);
    }

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT COUNT(*) 
            FROM incident.notifications 
            WHERE user_id = @userId AND is_read = FALSE";

        var count = await _db.ExecuteScalarAsync<long>(sql, ct, 
            new NpgsqlParameter("@userId", userId));

        return (int)count;
    }

    public async Task<NotificationType?> GetNotificationTypeByCodeAsync(string code, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT id, code, name, name_ar, description
            FROM incident.notification_types
            WHERE code = @code";

        await using var reader = await _db.ExecuteReaderAsync(sql, ct, 
            new NpgsqlParameter("@code", code));

        if (!await reader.ReadAsync(ct))
            return null;

        return new NotificationType
        {
            Id = reader.GetGuid(0),
            Code = reader.GetString(1),
            Name = reader.GetString(2),
            NameAr = reader.GetString(3),
            Description = reader.IsDBNull(4) ? null : reader.GetString(4)
        };
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        const string sql = @"
            DELETE FROM incident.notifications 
            WHERE id = @id AND user_id = @userId";

        var rows = await _db.ExecuteNonQueryAsync(sql, ct,
            new NpgsqlParameter("@id", id),
            new NpgsqlParameter("@userId", userId));

        return rows > 0;
    }

    private static Notification MapNotificationFromReader(NpgsqlDataReader reader)
    {
        return new Notification
        {
            Id = reader.GetGuid(0),
            UserId = reader.GetGuid(1),
            NotificationTypeId = reader.GetGuid(2),
            Title = reader.GetString(3),
            Message = reader.GetString(4),
            RelatedEntityType = reader.IsDBNull(5) ? null : reader.GetString(5),
            RelatedEntityId = reader.IsDBNull(6) ? null : reader.GetGuid(6),
            IsRead = reader.GetBoolean(7),
            ReadAt = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
            CreatedAt = reader.GetDateTime(9),
            Metadata = reader.IsDBNull(10) ? null : reader.GetString(10),
            NotificationType = new NotificationType
            {
                Id = reader.GetGuid(2),
                Code = reader.GetString(11),
                Name = reader.GetString(12),
                NameAr = reader.GetString(13)
            }
        };
    }
}
