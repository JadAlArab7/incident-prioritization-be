using System.Data;
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
        var results = new List<IncidentType>();
        using var connection = _dbHelper.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT id, name, description
            FROM incident.incident_types
            ORDER BY name";

        using var reader = await ExecuteReaderAsync(command);
        while (await ReadAsync(reader))
        {
            results.Add(new IncidentType
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description"))
            });
        }

        return results;
    }

    public async Task<IEnumerable<IncidentStatus>> GetIncidentStatusesAsync()
    {
        var results = new List<IncidentStatus>();
        using var connection = _dbHelper.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT id, code, name, description, is_terminal
            FROM incident.incident_statuses
            ORDER BY name";

        using var reader = await ExecuteReaderAsync(command);
        while (await ReadAsync(reader))
        {
            results.Add(new IncidentStatus
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Code = reader.GetString(reader.GetOrdinal("code")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                IsTerminal = reader.GetBoolean(reader.GetOrdinal("is_terminal"))
            });
        }

        return results;
    }

    public async Task<IEnumerable<Governorate>> GetGovernoratesAsync()
    {
        var results = new List<Governorate>();
        using var connection = _dbHelper.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT id, name, name_ar
            FROM incident.governorates
            ORDER BY name";

        using var reader = await ExecuteReaderAsync(command);
        while (await ReadAsync(reader))
        {
            results.Add(new Governorate
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                NameAr = reader.IsDBNull(reader.GetOrdinal("name_ar")) ? null : reader.GetString(reader.GetOrdinal("name_ar"))
            });
        }

        return results;
    }

    public async Task<IEnumerable<District>> GetDistrictsAsync()
    {
        var results = new List<District>();
        using var connection = _dbHelper.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT id, name, name_ar, governorate_id
            FROM incident.districts
            ORDER BY name";

        using var reader = await ExecuteReaderAsync(command);
        while (await ReadAsync(reader))
        {
            results.Add(new District
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                NameAr = reader.IsDBNull(reader.GetOrdinal("name_ar")) ? null : reader.GetString(reader.GetOrdinal("name_ar")),
                GovernorateId = reader.GetGuid(reader.GetOrdinal("governorate_id"))
            });
        }

        return results;
    }

    public async Task<IEnumerable<District>> GetDistrictsByGovernorateAsync(Guid governorateId)
    {
        var results = new List<District>();
        using var connection = _dbHelper.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT id, name, name_ar, governorate_id
            FROM incident.districts
            WHERE governorate_id = @GovernorateId
            ORDER BY name";

        var param = command.CreateParameter();
        param.ParameterName = "@GovernorateId";
        param.Value = governorateId;
        command.Parameters.Add(param);

        using var reader = await ExecuteReaderAsync(command);
        while (await ReadAsync(reader))
        {
            results.Add(new District
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                NameAr = reader.IsDBNull(reader.GetOrdinal("name_ar")) ? null : reader.GetString(reader.GetOrdinal("name_ar")),
                GovernorateId = reader.GetGuid(reader.GetOrdinal("governorate_id"))
            });
        }

        return results;
    }

    public async Task<IEnumerable<Town>> GetTownsAsync()
    {
        var results = new List<Town>();
        using var connection = _dbHelper.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT id, name, name_ar, district_id
            FROM incident.towns
            ORDER BY name";

        using var reader = await ExecuteReaderAsync(command);
        while (await ReadAsync(reader))
        {
            results.Add(new Town
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                NameAr = reader.IsDBNull(reader.GetOrdinal("name_ar")) ? null : reader.GetString(reader.GetOrdinal("name_ar")),
                DistrictId = reader.GetGuid(reader.GetOrdinal("district_id"))
            });
        }

        return results;
    }

    public async Task<IEnumerable<Town>> GetTownsByDistrictAsync(Guid districtId)
    {
        var results = new List<Town>();
        using var connection = _dbHelper.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT id, name, name_ar, district_id
            FROM incident.towns
            WHERE district_id = @DistrictId
            ORDER BY name";

        var param = command.CreateParameter();
        param.ParameterName = "@DistrictId";
        param.Value = districtId;
        command.Parameters.Add(param);

        using var reader = await ExecuteReaderAsync(command);
        while (await ReadAsync(reader))
        {
            results.Add(new Town
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                NameAr = reader.IsDBNull(reader.GetOrdinal("name_ar")) ? null : reader.GetString(reader.GetOrdinal("name_ar")),
                DistrictId = reader.GetGuid(reader.GetOrdinal("district_id"))
            });
        }

        return results;
    }

    public async Task<IEnumerable<Role>> GetRolesAsync()
    {
        var results = new List<Role>();
        using var connection = _dbHelper.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT id, name
            FROM incident.roles
            ORDER BY name";

        using var reader = await ExecuteReaderAsync(command);
        while (await ReadAsync(reader))
        {
            results.Add(new Role
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name"))
            });
        }

        return results;
    }

    private static async Task<IDataReader> ExecuteReaderAsync(IDbCommand command)
    {
        if (command is System.Data.Common.DbCommand dbCommand)
        {
            return await dbCommand.ExecuteReaderAsync();
        }
        return command.ExecuteReader();
    }

    private static async Task<bool> ReadAsync(IDataReader reader)
    {
        if (reader is System.Data.Common.DbDataReader dbReader)
        {
            return await dbReader.ReadAsync();
        }
        return reader.Read();
    }
}