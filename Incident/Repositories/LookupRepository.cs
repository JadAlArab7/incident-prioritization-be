using Incident.Infrastructure;
using Incident.Models;

namespace Incident.Repositories;

public class LookupRepository : ILookupRepository
{
    private readonly IDbHelper _dbHelper;

    public LookupRepository(IDbHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public async Task<IEnumerable<Governorate>> GetGovernoratesAsync()
    {
        const string sql = @"
            SELECT id, code, name_en as NameEn, name_ar as NameAr
            FROM governorates
            ORDER BY name_en";

        return await _dbHelper.QueryAsync<Governorate>(sql);
    }

    public async Task<IEnumerable<District>> GetDistrictsAsync(Guid governorateId)
    {
        const string sql = @"
            SELECT id, governorate_id as GovernorateId, code, name_en as NameEn, name_ar as NameAr
            FROM districts
            WHERE governorate_id = @GovernorateId
            ORDER BY name_en";

        return await _dbHelper.QueryAsync<District>(sql, new { GovernorateId = governorateId });
    }

    public async Task<IEnumerable<Town>> GetTownsAsync(Guid districtId)
    {
        const string sql = @"
            SELECT id, district_id as DistrictId, code, name_en as NameEn, name_ar as NameAr
            FROM towns
            WHERE district_id = @DistrictId
            ORDER BY name_en";

        return await _dbHelper.QueryAsync<Town>(sql, new { DistrictId = districtId });
    }

    public async Task<IEnumerable<IncidentType>> GetIncidentTypesAsync()
    {
        const string sql = @"
            SELECT id, code, name, description
            FROM incident_types
            ORDER BY name";

        return await _dbHelper.QueryAsync<IncidentType>(sql);
    }

    public async Task<IEnumerable<IncidentStatus>> GetIncidentStatusesAsync()
    {
        const string sql = @"
            SELECT id, code, name, description, is_terminal as IsTerminal
            FROM incident_statuses
            ORDER BY name";

        return await _dbHelper.QueryAsync<IncidentStatus>(sql);
    }

    public async Task<IEnumerable<User>> GetOfficersAsync()
    {
        const string sql = @"
            SELECT u.id, u.username, u.email, u.password_hash as PasswordHash, 
                   u.full_name as FullName, u.role_id as RoleId, u.is_active as IsActive,
                   u.created_at as CreatedAt, u.updated_at as UpdatedAt,
                   r.id as RoleId, r.code as Code, r.name as Name, r.description as Description
            FROM users u
            LEFT JOIN roles r ON u.role_id = r.id
            WHERE r.code = 'officer' AND u.is_active = true
            ORDER BY u.full_name";

        return await _dbHelper.QueryAsync<User, Role, User>(
            sql,
            (user, role) =>
            {
                if (role?.Id != Guid.Empty)
                {
                    user.Role = role;
                }
                return user;
            },
            splitOn: "RoleId"
        );
    }
}