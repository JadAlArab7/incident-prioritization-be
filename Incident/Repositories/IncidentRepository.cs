using Incident.Infrastructure;
using Incident.Models;
using Npgsql;

namespace Incident.Repositories;

public class IncidentRepository : IIncidentRepository
{
    private readonly IDbHelper _dbHelper;

    public IncidentRepository(IDbHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public async Task<IncidentRecord?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT i.id, i.title, i.description, i.incident_type_id, it.name as incident_type_name,
                   i.status_id, s.name as status_name, s.code as status_code,
                   i.location_id, i.created_by_user_id, cu.full_name as created_by_user_name,
                   i.sent_to_user_id, su.full_name as sent_to_user_name,
                   i.created_at, i.updated_at,
                   l.id as loc_id, l.governorate_id, g.name as governorate_name,
                   l.district_id, d.name as district_name, l.town_id, t.name as town_name
            FROM incident.incidents i
            LEFT JOIN incident.incident_types it ON i.incident_type_id = it.id
            LEFT JOIN incident.incident_statuses s ON i.status_id = s.id
            LEFT JOIN incident.users cu ON i.created_by_user_id = cu.id
            LEFT JOIN incident.users su ON i.sent_to_user_id = su.id
            LEFT JOIN incident.locations l ON i.location_id = l.id
            LEFT JOIN incident.governorates g ON l.governorate_id = g.id
            LEFT JOIN incident.districts d ON l.district_id = d.id
            LEFT JOIN incident.towns t ON l.town_id = t.id
            WHERE i.id = @id";

        var parameters = new[] { new NpgsqlParameter("@id", id) };

        return await _dbHelper.ExecuteReaderSingleAsync(sql, MapIncident, ct, parameters);
    }

    public async Task<List<IncidentRecord>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT i.id, i.title, i.description, i.incident_type_id, it.name as incident_type_name,
                   i.status_id, s.name as status_name, s.code as status_code,
                   i.location_id, i.created_by_user_id, cu.full_name as created_by_user_name,
                   i.sent_to_user_id, su.full_name as sent_to_user_name,
                   i.created_at, i.updated_at,
                   l.id as loc_id, l.governorate_id, g.name as governorate_name,
                   l.district_id, d.name as district_name, l.town_id, t.name as town_name
            FROM incident.incidents i
            LEFT JOIN incident.incident_types it ON i.incident_type_id = it.id
            LEFT JOIN incident.incident_statuses s ON i.status_id = s.id
            LEFT JOIN incident.users cu ON i.created_by_user_id = cu.id
            LEFT JOIN incident.users su ON i.sent_to_user_id = su.id
            LEFT JOIN incident.locations l ON i.location_id = l.id
            LEFT JOIN incident.governorates g ON l.governorate_id = g.id
            LEFT JOIN incident.districts d ON l.district_id = d.id
            LEFT JOIN incident.towns t ON l.town_id = t.id
            ORDER BY i.created_at DESC
            LIMIT @pageSize OFFSET @offset";

        var parameters = new[]
        {
            new NpgsqlParameter("@pageSize", pageSize),
            new NpgsqlParameter("@offset", (page - 1) * pageSize)
        };

        return await _dbHelper.ExecuteReaderAsync(sql, MapIncident, ct, parameters);
    }

    public async Task<int> GetTotalCountAsync(CancellationToken ct = default)
    {
        const string sql = "SELECT COUNT(*) FROM incident.incidents";
        var result = await _dbHelper.ExecuteScalarAsync<long>(sql, ct);
        return (int)result;
    }

    public async Task<Guid> CreateAsync(IncidentRecord incident, CancellationToken ct = default)
    {
        const string sql = @"
            INSERT INTO incident.incidents (id, title, description, incident_type_id, status_id, location_id, 
                                           created_by_user_id, sent_to_user_id, created_at, updated_at)
            VALUES (@id, @title, @description, @incidentTypeId, @statusId, @locationId, 
                    @createdByUserId, @sentToUserId, @createdAt, @updatedAt)
            RETURNING id";

        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var parameters = new[]
        {
            new NpgsqlParameter("@id", id),
            new NpgsqlParameter("@title", incident.Title),
            new NpgsqlParameter("@description", (object?)incident.Description ?? DBNull.Value),
            new NpgsqlParameter("@incidentTypeId", incident.IncidentTypeId),
            new NpgsqlParameter("@statusId", incident.StatusId),
            new NpgsqlParameter("@locationId", (object?)incident.LocationId ?? DBNull.Value),
            new NpgsqlParameter("@createdByUserId", incident.CreatedByUserId),
            new NpgsqlParameter("@sentToUserId", (object?)incident.SentToUserId ?? DBNull.Value),
            new NpgsqlParameter("@createdAt", now),
            new NpgsqlParameter("@updatedAt", now)
        };

        var result = await _dbHelper.ExecuteScalarAsync<Guid>(sql, ct, parameters);
        return result;
    }

    public async Task<bool> UpdateAsync(IncidentRecord incident, CancellationToken ct = default)
    {
        const string sql = @"
            UPDATE incident.incidents 
            SET title = @title, description = @description, incident_type_id = @incidentTypeId,
                status_id = @statusId, location_id = @locationId, sent_to_user_id = @sentToUserId,
                updated_at = @updatedAt
            WHERE id = @id";

        var parameters = new[]
        {
            new NpgsqlParameter("@id", incident.Id),
            new NpgsqlParameter("@title", incident.Title),
            new NpgsqlParameter("@description", (object?)incident.Description ?? DBNull.Value),
            new NpgsqlParameter("@incidentTypeId", incident.IncidentTypeId),
            new NpgsqlParameter("@statusId", incident.StatusId),
            new NpgsqlParameter("@locationId", (object?)incident.LocationId ?? DBNull.Value),
            new NpgsqlParameter("@sentToUserId", (object?)incident.SentToUserId ?? DBNull.Value),
            new NpgsqlParameter("@updatedAt", DateTime.UtcNow)
        };

        var rowsAffected = await _dbHelper.ExecuteNonQueryAsync(sql, ct, parameters);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = "DELETE FROM incident.incidents WHERE id = @id";
        var parameters = new[] { new NpgsqlParameter("@id", id) };

        var rowsAffected = await _dbHelper.ExecuteNonQueryAsync(sql, ct, parameters);
        return rowsAffected > 0;
    }

    private static IncidentRecord MapIncident(NpgsqlDataReader reader)
    {
        var incident = new IncidentRecord
        {
            Id = reader.GetGuid(0),
            Title = reader.GetString(1),
            Description = reader.IsDBNull(2) ? null : reader.GetString(2),
            IncidentTypeId = reader.GetGuid(3),
            IncidentTypeName = reader.IsDBNull(4) ? null : reader.GetString(4),
            StatusId = reader.GetGuid(5),
            StatusName = reader.IsDBNull(6) ? null : reader.GetString(6),
            StatusCode = reader.IsDBNull(7) ? null : reader.GetString(7),
            LocationId = reader.IsDBNull(8) ? null : reader.GetGuid(8),
            CreatedByUserId = reader.GetGuid(9),
            CreatedByUserName = reader.IsDBNull(10) ? null : reader.GetString(10),
            SentToUserId = reader.IsDBNull(11) ? null : reader.GetGuid(11),
            SentToUserName = reader.IsDBNull(12) ? null : reader.GetString(12),
            CreatedAt = reader.GetDateTime(13),
            UpdatedAt = reader.GetDateTime(14)
        };

        // Map location if present
        if (!reader.IsDBNull(15))
        {
            incident.Location = new Location
            {
                Id = reader.GetGuid(15),
                GovernorateId = reader.IsDBNull(16) ? null : reader.GetGuid(16),
                GovernorateName = reader.IsDBNull(17) ? null : reader.GetString(17),
                DistrictId = reader.IsDBNull(18) ? null : reader.GetGuid(18),
                DistrictName = reader.IsDBNull(19) ? null : reader.GetString(19),
                TownId = reader.IsDBNull(20) ? null : reader.GetGuid(20),
                TownName = reader.IsDBNull(21) ? null : reader.GetString(21)
            };
        }

        return incident;
    }
}