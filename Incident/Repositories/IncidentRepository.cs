using Incident.Infrastructure;
using Incident.Models;
using Incident.DTOs;

namespace Incident.Repositories;

public class IncidentRepository : IIncidentRepository
{
    private readonly IDbHelper _dbHelper;

    public IncidentRepository(IDbHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public async Task<IncidentRecord?> GetByIdAsync(Guid id)
    {
        const string sql = @"
            SELECT i.id, i.title, i.description, i.incident_type_id as IncidentTypeId, 
                   it.name as IncidentTypeName,
                   i.status_id as StatusId, 
                   s.name as StatusName,
                   i.location_id as LocationId,
                   i.created_by_user_id as CreatedByUserId,
                   cu.full_name as CreatedByUserName,
                   i.sent_to_user_id as SentToUserId,
                   su.full_name as SentToUserName,
                   i.created_at as CreatedAt, i.updated_at as UpdatedAt,
                   l.id as LocationId, l.governorate_id as GovernorateId, l.district_id as DistrictId, 
                   l.town_id as TownId, l.latitude, l.longitude, l.address_details as AddressDetails,
                   g.name_en as GovernorateName,
                   d.name_en as DistrictName,
                   t.name_en as TownName
            FROM incidents i
            LEFT JOIN incident_types it ON i.incident_type_id = it.id
            LEFT JOIN incident_statuses s ON i.status_id = s.id
            LEFT JOIN users cu ON i.created_by_user_id = cu.id
            LEFT JOIN users su ON i.sent_to_user_id = su.id
            LEFT JOIN locations l ON i.location_id = l.id
            LEFT JOIN governorates g ON l.governorate_id = g.id
            LEFT JOIN districts d ON l.district_id = d.id
            LEFT JOIN towns t ON l.town_id = t.id
            WHERE i.id = @Id";

        var result = await _dbHelper.QueryAsync<IncidentRecord, IncidentType, IncidentStatus, Location, Governorate, District, Town, User, User, IncidentRecord>(
            sql,
            (incident, incidentType, status, location, governorate, district, town, createdByUser, sentToUser) =>
            {
                incident.IncidentTypeName = incidentType?.Name;
                incident.StatusName = status?.Name;
                incident.CreatedByUserName = createdByUser?.FullName;
                incident.SentToUserName = sentToUser?.FullName;
                
                if (location != null)
                {
                    location.Governorate = governorate;
                    location.District = district;
                    location.Town = town;
                    incident.Location = location;
                }
                
                return incident;
            },
            new { Id = id },
            splitOn: "IncidentTypeName,StatusName,LocationId,GovernorateName,DistrictName,TownName,CreatedByUserName,SentToUserName"
        );

        return result.FirstOrDefault();
    }

    public async Task<PagedResponseDto<IncidentRecord>> GetPagedAsync(PagedRequestDto request)
    {
        // Build where clause
        var whereClause = "";
        var parameters = new Dictionary<string, object>();

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            whereClause += " AND (i.title ILIKE @SearchTerm OR i.description ILIKE @SearchTerm)";
            parameters.Add("SearchTerm", $"%{request.SearchTerm}%");
        }

        if (request.StatusId.HasValue)
        {
            whereClause += " AND i.status_id = @StatusId";
            parameters.Add("StatusId", request.StatusId.Value);
        }

        if (request.IncidentTypeId.HasValue)
        {
            whereClause += " AND i.incident_type_id = @IncidentTypeId";
            parameters.Add("IncidentTypeId", request.IncidentTypeId.Value);
        }

        if (request.GovernorateId.HasValue)
        {
            whereClause += " AND l.governorate_id = @GovernorateId";
            parameters.Add("GovernorateId", request.GovernorateId.Value);
        }

        if (request.DistrictId.HasValue)
        {
            whereClause += " AND l.district_id = @DistrictId";
            parameters.Add("DistrictId", request.DistrictId.Value);
        }

        if (request.TownId.HasValue)
        {
            whereClause += " AND l.town_id = @TownId";
            parameters.Add("TownId", request.TownId.Value);
        }

        if (request.CreatedByUserId.HasValue)
        {
            whereClause += " AND i.created_by_user_id = @CreatedByUserId";
            parameters.Add("CreatedByUserId", request.CreatedByUserId.Value);
        }

        if (request.SentToUserId.HasValue)
        {
            whereClause += " AND i.sent_to_user_id = @SentToUserId";
            parameters.Add("SentToUserId", request.SentToUserId.Value);
        }

        // Count query
        var countSql = $@"
            SELECT COUNT(1)
            FROM incidents i
            LEFT JOIN locations l ON i.location_id = l.id
            WHERE 1=1 {whereClause}";

        var totalCount = await _dbHelper.ExecuteScalarAsync<int>(countSql, parameters);

        // Data query
        var dataSql = $@"
            SELECT i.id, i.title, i.description, i.incident_type_id as IncidentTypeId, 
                   it.name as IncidentTypeName,
                   i.status_id as StatusId, 
                   s.name as StatusName,
                   i.location_id as LocationId,
                   i.created_by_user_id as CreatedByUserId,
                   cu.full_name as CreatedByUserName,
                   i.sent_to_user_id as SentToUserId,
                   su.full_name as SentToUserName,
                   i.created_at as CreatedAt, i.updated_at as UpdatedAt,
                   l.id as LocationId, l.governorate_id as GovernorateId, l.district_id as DistrictId, 
                   l.town_id as TownId, l.latitude, l.longitude, l.address_details as AddressDetails,
                   g.name_en as GovernorateName,
                   d.name_en as DistrictName,
                   t.name_en as TownName
            FROM incidents i
            LEFT JOIN incident_types it ON i.incident_type_id = it.id
            LEFT JOIN incident_statuses s ON i.status_id = s.id
            LEFT JOIN users cu ON i.created_by_user_id = cu.id
            LEFT JOIN users su ON i.sent_to_user_id = su.id
            LEFT JOIN locations l ON i.location_id = l.id
            LEFT JOIN governorates g ON l.governorate_id = g.id
            LEFT JOIN districts d ON l.district_id = d.id
            LEFT JOIN towns t ON l.town_id = t.id
            WHERE 1=1 {whereClause}
            ORDER BY i.created_at DESC
            LIMIT @Limit OFFSET @Offset";

        parameters.Add("Limit", request.PageSize);
        parameters.Add("Offset", (request.Page - 1) * request.PageSize);

        var data = await _dbHelper.QueryAsync<IncidentRecord, IncidentType, IncidentStatus, Location, Governorate, District, Town, User, User, IncidentRecord>(
            dataSql,
            (incident, incidentType, status, location, governorate, district, town, createdByUser, sentToUser) =>
            {
                incident.IncidentTypeName = incidentType?.Name;
                incident.StatusName = status?.Name;
                incident.CreatedByUserName = createdByUser?.FullName;
                incident.SentToUserName = sentToUser?.FullName;
                
                if (location != null)
                {
                    location.Governorate = governorate;
                    location.District = district;
                    location.Town = town;
                    incident.Location = location;
                }
                
                return incident;
            },
            parameters,
            splitOn: "IncidentTypeName,StatusName,LocationId,GovernorateName,DistrictName,TownName,CreatedByUserName,SentToUserName"
        );

        return new PagedResponseDto<IncidentRecord>
        {
            Data = data.ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<IncidentRecord> CreateAsync(IncidentRecord incident)
    {
        const string sql = @"
            INSERT INTO incidents 
                (id, title, description, incident_type_id, status_id, location_id, 
                 created_by_user_id, sent_to_user_id, created_at, updated_at)
            VALUES 
                (@Id, @Title, @Description, @IncidentTypeId, @StatusId, @LocationId, 
                 @CreatedByUserId, @SentToUserId, @CreatedAt, @UpdatedAt)
            RETURNING id, title, description, incident_type_id as IncidentTypeId, 
                      status_id as StatusId, location_id as LocationId, 
                      created_by_user_id as CreatedByUserId, sent_to_user_id as SentToUserId, 
                      created_at as CreatedAt, updated_at as UpdatedAt";

        incident.Id = Guid.NewGuid();
        incident.CreatedAt = DateTime.UtcNow;
        incident.UpdatedAt = DateTime.UtcNow;

        var createdIncident = await _dbHelper.QuerySingleOrDefaultAsync<IncidentRecord>(sql, new
        {
            incident.Id,
            incident.Title,
            incident.Description,
            incident.IncidentTypeId,
            incident.StatusId,
            incident.LocationId,
            incident.CreatedByUserId,
            incident.SentToUserId,
            incident.CreatedAt,
            incident.UpdatedAt
        });

        return createdIncident ?? incident;
    }

    public async Task<IncidentRecord?> UpdateAsync(IncidentRecord incident)
    {
        const string sql = @"
            UPDATE incidents 
            SET title = @Title, 
                description = @Description, 
                incident_type_id = @IncidentTypeId, 
                location_id = @LocationId, 
                sent_to_user_id = @SentToUserId, 
                updated_at = @UpdatedAt
            WHERE id = @Id
            RETURNING id, title, description, incident_type_id as IncidentTypeId, 
                      status_id as StatusId, location_id as LocationId, 
                      created_by_user_id as CreatedByUserId, sent_to_user_id as SentToUserId, 
                      created_at as CreatedAt, updated_at as UpdatedAt";

        incident.UpdatedAt = DateTime.UtcNow;

        return await _dbHelper.QuerySingleOrDefaultAsync<IncidentRecord>(sql, new
        {
            incident.Id,
            incident.Title,
            incident.Description,
            incident.IncidentTypeId,
            incident.LocationId,
            incident.SentToUserId,
            incident.UpdatedAt
        });
    }

    public async Task<bool> UpdateStatusAsync(Guid incidentId, Guid statusId, Guid? sentToUserId)
    {
        const string sql = @"
            UPDATE incidents 
            SET status_id = @StatusId, 
                sent_to_user_id = @SentToUserId, 
                updated_at = @UpdatedAt
            WHERE id = @IncidentId";

        var rowsAffected = await _dbHelper.ExecuteAsync(sql, new
        {
            IncidentId = incidentId,
            StatusId = statusId,
            SentToUserId = sentToUserId,
            UpdatedAt = DateTime.UtcNow
        });

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string sql = "DELETE FROM incidents WHERE id = @Id";
        var rowsAffected = await _dbHelper.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        const string sql = "SELECT COUNT(1) FROM incidents WHERE id = @Id";
        var count = await _dbHelper.ExecuteScalarAsync<int>(sql, new { Id = id });
        return count > 0;
    }
}