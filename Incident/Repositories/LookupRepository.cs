using Incident.Infrastructure;
using Incident.Models;
using Npgsql;

namespace Incident.Repositories;

public class LookupRepository : ILookupRepository
{
    private readonly IDbHelper _db;

    public LookupRepository(IDbHelper db)
    {
        _db = db;
    }

    public async Task<IEnumerable<User>> ListSecretariesAsync(CancellationToken ct = default)
    {
        const string sql = @"
            SELECT u.id, u.username, u.role_id, r.name as role_name
            FROM incident.users u
            JOIN incident.roles r ON u.role_id = r.id
            WHERE r.name = 'secretary'
            ORDER BY u.username";

        var users = new List<User>();
        await using var reader = await _db.ExecuteReaderAsync(sql, ct: ct);
        
        while (await reader.ReadAsync(ct))
        {
            users.Add(new User
            {
                Id = reader.GetGuid(0),
                Username = reader.GetString(1),
                RoleId = reader.GetGuid(2),
                Role = new Role
                {
                    Id = reader.GetGuid(2),
                    Name = reader.GetString(3)
                }
            });
        }

        return users;
    }

    public async Task<IEnumerable<IncidentType>> ListIncidentTypesAsync(CancellationToken ct = default)
    {
        const string sql = @"
            SELECT id, name, name_en, name_ar
            FROM incident.incident_types
            ORDER BY name_en";

        var types = new List<IncidentType>();
        await using var reader = await _db.ExecuteReaderAsync(sql, ct: ct);
        
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

    public async Task<IEnumerable<IncidentStatus>> ListIncidentStatusesAsync(CancellationToken ct = default)
    {
        const string sql = @"
            SELECT id, code, name, name_ar
            FROM incident.incident_statuses
            ORDER BY code";

        var statuses = new List<IncidentStatus>();
        await using var reader = await _db.ExecuteReaderAsync(sql, ct: ct);
        
        while (await reader.ReadAsync(ct))
        {
            statuses.Add(new IncidentStatus
            {
                Id = reader.GetGuid(0),
                Code = reader.GetString(1),
                Name = reader.GetString(2),
                NameAr = reader.GetString(3)
            });
        }

        return statuses;
    }

    public async Task<IEnumerable<Governorate>> ListGovernoratesAsync(CancellationToken ct = default)
    {
        const string sql = @"
            SELECT id, name, name_ar, created_at
            FROM incident.governorates
            ORDER BY name";

        var governorates = new List<Governorate>();
        await using var reader = await _db.ExecuteReaderAsync(sql, ct: ct);
        
        while (await reader.ReadAsync(ct))
        {
            governorates.Add(new Governorate
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                NameAr = reader.GetString(2),
                CreatedAt = reader.GetDateTime(3)
            });
        }

        return governorates;
    }

    public async Task<IEnumerable<District>> ListDistrictsAsync(Guid governorateId, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT id, governorate_id, name, name_ar, created_at
            FROM incident.districts
            WHERE governorate_id = @governorateId
            ORDER BY name";

        var districts = new List<District>();
        await using var reader = await _db.ExecuteReaderAsync(sql, 
            ct: ct,
            parameters: new NpgsqlParameter("@governorateId", governorateId));
        
        while (await reader.ReadAsync(ct))
        {
            districts.Add(new District
            {
                Id = reader.GetGuid(0),
                GovernorateId = reader.GetGuid(1),
                Name = reader.GetString(2),
                NameAr = reader.GetString(3),
                CreatedAt = reader.GetDateTime(4)
            });
        }

        return districts;
    }

    public async Task<IEnumerable<Town>> ListTownsAsync(Guid districtId, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT id, district_id, name, name_ar, lat, lng, created_at
            FROM incident.towns
            WHERE district_id = @districtId
            ORDER BY name";

        var towns = new List<Town>();
        await using var reader = await _db.ExecuteReaderAsync(sql, 
            ct: ct,
            parameters: new NpgsqlParameter("@districtId", districtId));
        
        while (await reader.ReadAsync(ct))
        {
            towns.Add(new Town
            {
                Id = reader.GetGuid(0),
                DistrictId = reader.GetGuid(1),
                Name = reader.GetString(2),
                NameAr = reader.GetString(3),
                Lat = reader.IsDBNull(4) ? null : reader.GetDouble(4),
                Lng = reader.IsDBNull(5) ? null : reader.GetDouble(5),
                CreatedAt = reader.GetDateTime(6)
            });
        }

        return towns;
    }

    public async Task<IncidentStatus?> GetStatusByCodeAsync(string code, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT id, code, name, name_ar
            FROM incident.incident_statuses
            WHERE code = @code";

        await using var reader = await _db.ExecuteReaderAsync(sql, 
            ct: ct,
            parameters: new NpgsqlParameter("@code", code));
        
        if (await reader.ReadAsync(ct))
        {
            return new IncidentStatus
            {
                Id = reader.GetGuid(0),
                Code = reader.GetString(1),
                Name = reader.GetString(2),
                NameAr = reader.GetString(3)
            };
        }

        return null;
    }

    public async Task<IncidentStatus?> GetStatusByIdAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT id, code, name, name_ar
            FROM incident.incident_statuses
            WHERE id = @id";

        await using var reader = await _db.ExecuteReaderAsync(sql, 
            ct: ct,
            parameters: new NpgsqlParameter("@id", id));
        
        if (await reader.ReadAsync(ct))
        {
            return new IncidentStatus
            {
                Id = reader.GetGuid(0),
                Code = reader.GetString(1),
                Name = reader.GetString(2),
                NameAr = reader.GetString(3)
            };
        }

        return null;
    }
}