using Incident.Infrastructure;
using Incident.Models;
using Npgsql;

namespace Incident.Repositories;

public class IncidentStatusRepository : IIncidentStatusRepository
{
    private readonly IDbHelper _dbHelper;

    public IncidentStatusRepository(IDbHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public async Task<IncidentStatusTransition?> GetTransitionAsync(Guid fromStatusId, string actionCode, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT id, from_status_id, to_status_id, action_code, initiator, is_active
            FROM incident.incident_status_transitions
            WHERE from_status_id = @fromStatusId 
              AND action_code = @actionCode 
              AND is_active = TRUE";

        var parameters = new[]
        {
            new NpgsqlParameter("@fromStatusId", fromStatusId),
            new NpgsqlParameter("@actionCode", actionCode)
        };

        return await _dbHelper.ExecuteReaderSingleAsync(sql, MapTransition, ct, parameters);
    }

    public async Task<List<IncidentStatusTransition>> GetTransitionsFromStatusAsync(Guid statusId, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT id, from_status_id, to_status_id, action_code, initiator, is_active
            FROM incident.incident_status_transitions
            WHERE from_status_id = @statusId AND is_active = TRUE";

        var parameters = new[] { new NpgsqlParameter("@statusId", statusId) };

        return await _dbHelper.ExecuteReaderAsync(sql, MapTransition, ct, parameters);
    }

    public async Task<Guid?> GetStatusIdByCodeAsync(string code, CancellationToken ct = default)
    {
        const string sql = "SELECT id FROM incident.incident_statuses WHERE code = @code";
        var parameters = new[] { new NpgsqlParameter("@code", code) };

        return await _dbHelper.ExecuteScalarAsync<Guid?>(sql, ct, parameters);
    }

    public async Task<string?> GetStatusCodeByIdAsync(Guid statusId, CancellationToken ct = default)
    {
        const string sql = "SELECT code FROM incident.incident_statuses WHERE id = @statusId";
        var parameters = new[] { new NpgsqlParameter("@statusId", statusId) };

        return await _dbHelper.ExecuteScalarAsync<string?>(sql, ct, parameters);
    }

    public async Task InsertStatusHistoryAsync(IncidentStatusHistory history, CancellationToken ct = default)
    {
        const string sql = @"
            INSERT INTO incident.incident_status_history (id, incident_id, from_status_id, to_status_id, changed_by_user_id, comment, changed_at)
            VALUES (@id, @incidentId, @fromStatusId, @toStatusId, @changedByUserId, @comment, @changedAt)";

        var parameters = new[]
        {
            new NpgsqlParameter("@id", history.Id),
            new NpgsqlParameter("@incidentId", history.IncidentId),
            new NpgsqlParameter("@fromStatusId", history.FromStatusId),
            new NpgsqlParameter("@toStatusId", history.ToStatusId),
            new NpgsqlParameter("@changedByUserId", history.ChangedByUserId),
            new NpgsqlParameter("@comment", (object?)history.Comment ?? DBNull.Value),
            new NpgsqlParameter("@changedAt", history.ChangedAt)
        };

        await _dbHelper.ExecuteNonQueryAsync(sql, ct, parameters);
    }

    public async Task<List<IncidentStatusHistory>> GetStatusHistoryAsync(Guid incidentId, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT id, incident_id, from_status_id, to_status_id, changed_by_user_id, comment, changed_at
            FROM incident.incident_status_history
            WHERE incident_id = @incidentId
            ORDER BY changed_at DESC";

        var parameters = new[] { new NpgsqlParameter("@incidentId", incidentId) };

        return await _dbHelper.ExecuteReaderAsync(sql, MapHistory, ct, parameters);
    }

    private static IncidentStatusTransition MapTransition(NpgsqlDataReader reader)
    {
        return new IncidentStatusTransition
        {
            Id = reader.GetGuid(0),
            FromStatusId = reader.GetGuid(1),
            ToStatusId = reader.GetGuid(2),
            ActionCode = reader.GetString(3),
            Initiator = reader.GetString(4),
            IsActive = reader.GetBoolean(5)
        };
    }

    private static IncidentStatusHistory MapHistory(NpgsqlDataReader reader)
    {
        return new IncidentStatusHistory
        {
            Id = reader.GetGuid(0),
            IncidentId = reader.GetGuid(1),
            FromStatusId = reader.GetGuid(2),
            ToStatusId = reader.GetGuid(3),
            ChangedByUserId = reader.GetGuid(4),
            Comment = reader.IsDBNull(5) ? null : reader.GetString(5),
            ChangedAt = reader.GetDateTime(6)
        };
    }
}