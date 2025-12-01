using Incident.Infrastructure;
using Incident.Models;
using Npgsql;

namespace Incident.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly IDbHelper _dbHelper;

    public RoleRepository(IDbHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public async Task<Role?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = "SELECT id, code, name FROM incident.roles WHERE id = @id";
        var parameters = new[] { new NpgsqlParameter("@id", id) };

        return await _dbHelper.ExecuteReaderSingleAsync(sql, MapRole, ct, parameters);
    }

    public async Task<Role?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        const string sql = "SELECT id, code, name FROM incident.roles WHERE code = @code";
        var parameters = new[] { new NpgsqlParameter("@code", code) };

        return await _dbHelper.ExecuteReaderSingleAsync(sql, MapRole, ct, parameters);
    }

    public async Task<List<Role>> GetAllAsync(CancellationToken ct = default)
    {
        const string sql = "SELECT id, code, name FROM incident.roles ORDER BY name";
        return await _dbHelper.ExecuteReaderAsync(sql, MapRole, ct);
    }

    private static Role MapRole(NpgsqlDataReader reader)
    {
        return new Role
        {
            Id = reader.GetGuid(0),
            Code = reader.GetString(1),
            Name = reader.GetString(2)
        };
    }
}