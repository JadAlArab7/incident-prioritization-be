using Incident.Infrastructure;
using Incident.Models;
using Npgsql;

namespace Incident.Repositories;

public class LookupRepository : ILookupRepository
{
    private readonly IDbHelper _dbHelper;

    public LookupRepository(IDbHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public async Task<List<Governorate>> GetGovernoratesAsync(CancellationToken ct = default)
    {
        const string sql = "SELECT id, name, code FROM incident.governorates ORDER BY name";
        return await _dbHelper.ExecuteReaderAsync(sql, reader => new Governorate
        {
            Id = reader.GetGuid(0),
            Name = reader.GetString(1),
            Code = reader.IsDBNull(2) ? null : reader.GetString(2)
        }, ct);
    }

    public async Task<List<District>> GetDistrictsByGovernorateAsync(Guid governorateId, CancellationToken ct = default)
    {
        const string sql = "SELECT id, name, code, governorate_id FROM incident.districts WHERE governorate_id = @governorateId ORDER BY name";
        var parameters = new[] { new NpgsqlParameter("@governorateId", governorateId) };

        return await _dbHelper.ExecuteReaderAsync(sql, reader => new District
        {
            Id = reader.GetGuid(0),
            Name = reader.GetString(1),
            Code = reader.IsDBNull(2) ? null : reader.GetString(2),
            GovernorateId = reader.GetGuid(3)
        }, ct, parameters);
    }

    public async Task<List<Town>> GetTownsByDistrictAsync(Guid districtId, CancellationToken ct = default)
    {
        const string sql = "SELECT id, name, code, district_id FROM incident.towns WHERE district_id = @districtId ORDER BY name";
        var parameters = new[] { new NpgsqlParameter("@districtId", districtId) };

        return await _dbHelper.ExecuteReaderAsync(sql, reader => new Town
        {
            Id = reader.GetGuid(0),
            Name = reader.GetString(1),
            Code = reader.IsDBNull(2) ? null : reader.GetString(2),
            DistrictId = reader.GetGuid(3)
        }, ct, parameters);
    }

    public async Task<List<IncidentType>> GetIncidentTypesAsync(CancellationToken ct = default)
    {
        const string sql = "SELECT id, name, code FROM incident.incident_types ORDER BY name";
        return await _dbHelper.ExecuteReaderAsync(sql, reader => new IncidentType
        {
            Id = reader.GetGuid(0),
            Name = reader.GetString(1),
            Code = reader.IsDBNull(2) ? null : reader.GetString(2)
        }, ct);
    }

    public async Task<List<IncidentStatus>> GetIncidentStatusesAsync(CancellationToken ct = default)
    {
        const string sql = "SELECT id, name, code FROM incident.incident_statuses ORDER BY name";
        return await _dbHelper.ExecuteReaderAsync(sql, reader => new IncidentStatus
        {
            Id = reader.GetGuid(0),
            Name = reader.GetString(1),
            Code = reader.IsDBNull(2) ? null : reader.GetString(2)
        }, ct);
    }
}