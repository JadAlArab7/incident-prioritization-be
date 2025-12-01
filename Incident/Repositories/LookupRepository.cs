using Dapper;
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

    public async Task<IEnumerable<IncidentType>> GetIncidentTypesAsync()
    {
        using var connection = _dbHelper.CreateConnection();
        const string sql = @"
            SELECT id, name, description
            FROM incident.incident_types
            ORDER BY name";

        return await connection.QueryAsync<IncidentType>(sql);
    }

    public async Task<IEnumerable<IncidentStatus>> GetIncidentStatusesAsync()
    {
        using var connection = _dbHelper.CreateConnection();
        const string sql = @"
            SELECT id, code, name, description, is_terminal as IsTerminal
            FROM incident.incident_statuses
            ORDER BY name";

        return await connection.QueryAsync<IncidentStatus>(sql);
    }

    public async Task<IEnumerable<Governorate>> GetGovernoratesAsync()
    {
        using var connection = _dbHelper.CreateConnection();
        const string sql = @"
            SELECT id, name, name_ar as NameAr
            FROM incident.governorates
            ORDER BY name";

        return await connection.QueryAsync<Governorate>(sql);
    }

    public async Task<IEnumerable<District>> GetDistrictsAsync()
    {
        using var connection = _dbHelper.CreateConnection();
        const string sql = @"
            SELECT id, name, name_ar as NameAr, governorate_id as GovernorateId
            FROM incident.districts
            ORDER BY name";

        return await connection.QueryAsync<District>(sql);
    }

    public async Task<IEnumerable<District>> GetDistrictsByGovernorateAsync(Guid governorateId)
    {
        using var connection = _dbHelper.CreateConnection();
        const string sql = @"
            SELECT id, name, name_ar as NameAr, governorate_id as GovernorateId
            FROM incident.districts
            WHERE governorate_id = @GovernorateId
            ORDER BY name";

        return await connection.QueryAsync<District>(sql, new { GovernorateId = governorateId });
    }

    public async Task<IEnumerable<Town>> GetTownsAsync()
    {
        using var connection = _dbHelper.CreateConnection();
        const string sql = @"
            SELECT id, name, name_ar as NameAr, district_id as DistrictId
            FROM incident.towns
            ORDER BY name";

        return await connection.QueryAsync<Town>(sql);
    }

    public async Task<IEnumerable<Town>> GetTownsByDistrictAsync(Guid districtId)
    {
        using var connection = _dbHelper.CreateConnection();
        const string sql = @"
            SELECT id, name, name_ar as NameAr, district_id as DistrictId
            FROM incident.towns
            WHERE district_id = @DistrictId
            ORDER BY name";

        return await connection.QueryAsync<Town>(sql, new { DistrictId = districtId });
    }

    public async Task<IEnumerable<Role>> GetRolesAsync()
    {
        using var connection = _dbHelper.CreateConnection();
        const string sql = @"
            SELECT id, name
            FROM incident.roles
            ORDER BY name";

        return await connection.QueryAsync<Role>(sql);
    }
}