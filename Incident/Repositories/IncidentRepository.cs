using Incident.Infrastructure;
using Incident.Models;
using Npgsql;

namespace Incident.Repositories;

public class IncidentRepository : IIncidentRepository
{
    private readonly IDbHelper _db;

    public IncidentRepository(IDbHelper db)
    {
        _db = db;
    }

    public async Task<Guid> CreateLocationAsync(Location location, CancellationToken ct = default)
    {
        const string sql = @"
            INSERT INTO incident.locations (id, lat, lng, governorate_id, district_id, town_id, address_text)
            VALUES (@id, @lat, @lng, @governorateId, @districtId, @townId, @addressText)
            RETURNING id";

        var id = Guid.NewGuid();
        await _db.ExecuteScalarAsync<Guid>(sql, ct,
            new NpgsqlParameter("@id", id),
            new NpgsqlParameter("@lat", location.Lat),
            new NpgsqlParameter("@lng", location.Lng),
            new NpgsqlParameter("@governorateId", (object?)location.GovernorateId ?? DBNull.Value),
            new NpgsqlParameter("@districtId", (object?)location.DistrictId ?? DBNull.Value),
            new NpgsqlParameter("@townId", (object?)location.TownId ?? DBNull.Value),
            new NpgsqlParameter("@addressText", (object?)location.AddressText ?? DBNull.Value));

        return id;
    }

    public async Task<bool> UpdateLocationAsync(Location location, CancellationToken ct = default)
    {
        const string sql = @"
            UPDATE incident.locations
            SET lat = @lat, lng = @lng, governorate_id = @governorateId, 
                district_id = @districtId, town_id = @townId, address_text = @addressText
            WHERE id = @id";

        var rows = await _db.ExecuteNonQueryAsync(sql, ct,
            new NpgsqlParameter("@id", location.Id),
            new NpgsqlParameter("@lat", location.Lat),
            new NpgsqlParameter("@lng", location.Lng),
            new NpgsqlParameter("@governorateId", (object?)location.GovernorateId ?? DBNull.Value),
            new NpgsqlParameter("@districtId", (object?)location.DistrictId ?? DBNull.Value),
            new NpgsqlParameter("@townId", (object?)location.TownId ?? DBNull.Value),
            new NpgsqlParameter("@addressText", (object?)location.AddressText ?? DBNull.Value));

        return rows > 0;
    }

    public async Task<Guid> CreateAsync(IncidentRecord entity, IEnumerable<Guid> typeIds, CancellationToken ct = default)
    {
        var id = Guid.NewGuid();
        
        const string insertIncidentSql = @"
            INSERT INTO incident.incidents 
                (id, title, description, sent_to_user_id, created_by_user_id, location_id, 
                 priority, suggested_actions_taken, status_id)
            VALUES 
                (@id, @title, @description, @sentToUserId, @createdByUserId, @locationId,
                 @priority, @suggestedActionsTaken, @statusId)";

        await _db.ExecuteNonQueryAsync(insertIncidentSql, ct,
            new NpgsqlParameter("@id", id),
            new NpgsqlParameter("@title", entity.Title),
            new NpgsqlParameter("@description", (object?)entity.Description ?? DBNull.Value),
            new NpgsqlParameter("@sentToUserId", (object?)entity.SentToUserId ?? DBNull.Value),
            new NpgsqlParameter("@createdByUserId", entity.CreatedByUserId),
            new NpgsqlParameter("@locationId", (object?)entity.LocationId ?? DBNull.Value),
            new NpgsqlParameter("@priority", (object?)entity.Priority ?? DBNull.Value),
            new NpgsqlParameter("@suggestedActionsTaken", (object?)entity.SuggestedActionsTaken ?? DBNull.Value),
            new NpgsqlParameter("@statusId", entity.StatusId));

        // Insert incident types
        foreach (var typeId in typeIds)
        {
            const string insertTypeSql = @"
                INSERT INTO incident.incident_incident_types (incident_id, incident_type_id)
                VALUES (@incidentId, @typeId)
                ON CONFLICT DO NOTHING";

            await _db.ExecuteNonQueryAsync(insertTypeSql, ct,
                new NpgsqlParameter("@incidentId", id),
                new NpgsqlParameter("@typeId", typeId));
        }

        return id;
    }

    public async Task<bool> UpdateAsync(IncidentRecord entity, IEnumerable<Guid> typeIds, CancellationToken ct = default)
    {
        const string updateSql = @"
            UPDATE incident.incidents
            SET title = @title, description = @description, sent_to_user_id = @sentToUserId,
                location_id = @locationId, priority = @priority, 
                suggested_actions_taken = @suggestedActionsTaken, status_id = @statusId
            WHERE id = @id";

        var rows = await _db.ExecuteNonQueryAsync(updateSql, ct,
            new NpgsqlParameter("@id", entity.Id),
            new NpgsqlParameter("@title", entity.Title),
            new NpgsqlParameter("@description", (object?)entity.Description ?? DBNull.Value),
            new NpgsqlParameter("@sentToUserId", (object?)entity.SentToUserId ?? DBNull.Value),
            new NpgsqlParameter("@locationId", (object?)entity.LocationId ?? DBNull.Value),
            new NpgsqlParameter("@priority", (object?)entity.Priority ?? DBNull.Value),
            new NpgsqlParameter("@suggestedActionsTaken", (object?)entity.SuggestedActionsTaken ?? DBNull.Value),
            new NpgsqlParameter("@statusId", entity.StatusId));

        if (rows == 0) return false;

        // Delete existing types and re-insert
        const string deleteTypesSql = "DELETE FROM incident.incident_incident_types WHERE incident_id = @incidentId";
        await _db.ExecuteNonQueryAsync(deleteTypesSql, ct, new NpgsqlParameter("@incidentId", entity.Id));

        foreach (var typeId in typeIds)
        {
            const string insertTypeSql = @"
                INSERT INTO incident.incident_incident_types (incident_id, incident_type_id)
                VALUES (@incidentId, @typeId)";

            await _db.ExecuteNonQueryAsync(insertTypeSql, ct,
                new NpgsqlParameter("@incidentId", entity.Id),
                new NpgsqlParameter("@typeId", typeId));
        }

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        // First get the location_id to delete it after
        const string getLocationSql = "SELECT location_id FROM incident.incidents WHERE id = @id";
        var locationId = await _db.ExecuteScalarAsync<Guid?>(getLocationSql, ct, new NpgsqlParameter("@id", id));

        const string deleteSql = "DELETE FROM incident.incidents WHERE id = @id";
        var rows = await _db.ExecuteNonQueryAsync(deleteSql, ct, new NpgsqlParameter("@id", id));

        // Clean up orphaned location
        if (locationId.HasValue)
        {
            const string deleteLocationSql = "DELETE FROM incident.locations WHERE id = @locationId";
            await _db.ExecuteNonQueryAsync(deleteLocationSql, ct, new NpgsqlParameter("@locationId", locationId.Value));
        }

        return rows > 0;
    }

    public async Task<IncidentRecord?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT 
                i.id, i.title, i.description, i.sent_to_user_id, i.created_by_user_id,
                i.location_id, i.priority, i.suggested_actions_taken, i.status_id,
                i.created_at, i.updated_at,
                -- Status
                s.code as status_code, s.name as status_name, s.name_ar as status_name_ar,
                -- Created by user
                cu.username as created_by_username, cr.name as created_by_role,
                -- Sent to user
                su.username as sent_to_username, sr.name as sent_to_role,
                -- Location
                l.lat, l.lng, l.address_text,
                l.governorate_id, l.district_id, l.town_id,
                g.name as gov_name, g.name_ar as gov_name_ar,
                d.name as dist_name, d.name_ar as dist_name_ar,
                t.name as town_name, t.name_ar as town_name_ar
            FROM incident.incidents i
            JOIN incident.incident_statuses s ON i.status_id = s.id
            JOIN incident.users cu ON i.created_by_user_id = cu.id
            JOIN incident.roles cr ON cu.role_id = cr.id
            LEFT JOIN incident.users su ON i.sent_to_user_id = su.id
            LEFT JOIN incident.roles sr ON su.role_id = sr.id
            LEFT JOIN incident.locations l ON i.location_id = l.id
            LEFT JOIN incident.governorates g ON l.governorate_id = g.id
            LEFT JOIN incident.districts d ON l.district_id = d.id
            LEFT JOIN incident.towns t ON l.town_id = t.id
            WHERE i.id = @id";

        await using var reader = await _db.ExecuteReaderAsync(sql, ct, new NpgsqlParameter("@id", id));

        if (!await reader.ReadAsync(ct))
            return null;

        var incident = MapIncidentFromReader(reader);

        // Get types
        await reader.CloseAsync();
        incident.Types = (await GetIncidentTypesAsync(id, ct)).ToList();

        return incident;
    }

    public async Task<(IEnumerable<IncidentRecord> Items, int TotalCount)> ListForUserAsync(
        Guid userId, int page, int pageSize, CancellationToken ct = default)
    {
        // Show incidents created by user OR assigned to user (but not in draft status)
        var whereClause = @"WHERE (i.created_by_user_id = @userId) 
                             OR (i.sent_to_user_id = @userId AND s.code != 'draft')";

        var countSql = $@"
            SELECT COUNT(*) 
            FROM incident.incidents i 
            JOIN incident.incident_statuses s ON i.status_id = s.id
            {whereClause}";

        var totalCount = await _db.ExecuteScalarAsync<long>(countSql, ct, new NpgsqlParameter("@userId", userId));

        var sql = $@"
            SELECT 
                i.id, i.title, i.description, i.sent_to_user_id, i.created_by_user_id,
                i.location_id, i.priority, i.suggested_actions_taken, i.status_id,
                i.created_at, i.updated_at,
                s.code as status_code, s.name as status_name, s.name_ar as status_name_ar,
                cu.username as created_by_username, cr.name as created_by_role,
                su.username as sent_to_username, sr.name as sent_to_role,
                l.lat, l.lng, l.address_text,
                l.governorate_id, l.district_id, l.town_id,
                g.name as gov_name, g.name_ar as gov_name_ar,
                d.name as dist_name, d.name_ar as dist_name_ar,
                t.name as town_name, t.name_ar as town_name_ar
            FROM incident.incidents i
            JOIN incident.incident_statuses s ON i.status_id = s.id
            JOIN incident.users cu ON i.created_by_user_id = cu.id
            JOIN incident.roles cr ON cu.role_id = cr.id
            LEFT JOIN incident.users su ON i.sent_to_user_id = su.id
            LEFT JOIN incident.roles sr ON su.role_id = sr.id
            LEFT JOIN incident.locations l ON i.location_id = l.id
            LEFT JOIN incident.governorates g ON l.governorate_id = g.id
            LEFT JOIN incident.districts d ON l.district_id = d.id
            LEFT JOIN incident.towns t ON l.town_id = t.id
            {whereClause}
            ORDER BY i.created_at DESC
            LIMIT @pageSize OFFSET @offset";

        var incidents = new List<IncidentRecord>();
        await using var reader = await _db.ExecuteReaderAsync(sql, ct,
            new NpgsqlParameter("@userId", userId),
            new NpgsqlParameter("@pageSize", pageSize),
            new NpgsqlParameter("@offset", (page - 1) * pageSize));

        while (await reader.ReadAsync(ct))
        {
            incidents.Add(MapIncidentFromReader(reader));
        }

        await reader.CloseAsync();

        // Load types for each incident
        foreach (var incident in incidents)
        {
            incident.Types = (await GetIncidentTypesAsync(incident.Id, ct)).ToList();
        }

        return (incidents, (int)totalCount);
    }

    private async Task<IEnumerable<IncidentType>> GetIncidentTypesAsync(Guid incidentId, CancellationToken ct)
    {
        const string sql = @"
            SELECT it.id, it.name, it.name_en, it.name_ar
            FROM incident.incident_types it
            JOIN incident.incident_incident_types iit ON it.id = iit.incident_type_id
            WHERE iit.incident_id = @incidentId";

        var types = new List<IncidentType>();
        await using var reader = await _db.ExecuteReaderAsync(sql, ct, new NpgsqlParameter("@incidentId", incidentId));

        while (await reader.ReadAsync(ct))
        {
            types.Add(new IncidentType
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                NameEn = reader.GetString(2),
                NameAr = reader.GetString(3)
            });
        }

        return types;
    }

    private static IncidentRecord MapIncidentFromReader(NpgsqlDataReader reader)
    {
        var incident = new IncidentRecord
        {
            Id = reader.GetGuid(0),
            Title = reader.GetString(1),
            Description = reader.IsDBNull(2) ? null : reader.GetString(2),
            SentToUserId = reader.IsDBNull(3) ? null : reader.GetGuid(3),
            CreatedByUserId = reader.GetGuid(4),
            LocationId = reader.IsDBNull(5) ? null : reader.GetGuid(5),
            Priority = reader.IsDBNull(6) ? null : reader.GetString(6),
            SuggestedActionsTaken = reader.IsDBNull(7) ? null : reader.GetString(7),
            StatusId = reader.GetGuid(8),
            CreatedAt = reader.GetDateTime(9),
            UpdatedAt = reader.GetDateTime(10),
            Status = new IncidentStatus
            {
                Id = reader.GetGuid(8),
                Code = reader.GetString(11),
                Name = reader.GetString(12),
                NameAr = reader.GetString(13)
            },
            CreatedByUser = new User
            {
                Id = reader.GetGuid(4),
                Username = reader.GetString(14),
                Role = new Role { Name = reader.GetString(15) }
            }
        };

        // Sent to user
        if (!reader.IsDBNull(3))
        {
            incident.SentToUser = new User
            {
                Id = reader.GetGuid(3),
                Username = reader.IsDBNull(16) ? string.Empty : reader.GetString(16),
                Role = new Role { Name = reader.IsDBNull(17) ? string.Empty : reader.GetString(17) }
            };
        }

        // Location
        if (!reader.IsDBNull(5))
        {
            incident.Location = new Location
            {
                Id = reader.GetGuid(5),
                Lat = reader.IsDBNull(18) ? 0 : reader.GetDouble(18),
                Lng = reader.IsDBNull(19) ? 0 : reader.GetDouble(19),
                AddressText = reader.IsDBNull(20) ? null : reader.GetString(20),
                GovernorateId = reader.IsDBNull(21) ? null : reader.GetGuid(21),
                DistrictId = reader.IsDBNull(22) ? null : reader.GetGuid(22),
                TownId = reader.IsDBNull(23) ? null : reader.GetGuid(23)
            };

            if (!reader.IsDBNull(21))
            {
                incident.Location.Governorate = new Governorate
                {
                    Id = reader.GetGuid(21),
                    Name = reader.IsDBNull(24) ? string.Empty : reader.GetString(24),
                    NameAr = reader.IsDBNull(25) ? string.Empty : reader.GetString(25)
                };
            }

            if (!reader.IsDBNull(22))
            {
                incident.Location.District = new District
                {
                    Id = reader.GetGuid(22),
                    Name = reader.IsDBNull(26) ? string.Empty : reader.GetString(26),
                    NameAr = reader.IsDBNull(27) ? string.Empty : reader.GetString(27)
                };
            }

            if (!reader.IsDBNull(23))
            {
                incident.Location.Town = new Town
                {
                    Id = reader.GetGuid(23),
                    Name = reader.IsDBNull(28) ? string.Empty : reader.GetString(28),
                    NameAr = reader.IsDBNull(29) ? string.Empty : reader.GetString(29)
                };
            }
        }

        return incident;
    }

    public async Task<bool> UpdateStatusAsync(Guid incidentId, Guid statusId, CancellationToken ct = default)
    {
        const string sql = @"
            UPDATE incident.incidents
            SET status_id = @statusId
            WHERE id = @incidentId";

        var rows = await _db.ExecuteNonQueryAsync(sql, ct,
            new NpgsqlParameter("@incidentId", incidentId),
            new NpgsqlParameter("@statusId", statusId));

        return rows > 0;
    }
}