using Incident.Infrastructure;
using Incident.Models;
using Npgsql;
using NpgsqlTypes;

namespace Incident.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly IDbHelper _dbHelper;

    public RoleRepository(IDbHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        const string sql = "SELECT id, name FROM roles WHERE name = @name";

        return await _dbHelper.QuerySingleOrDefaultAsync(
            sql,
            MapRole,
            new NpgsqlParameter("@name", NpgsqlDbType.Text) { Value = name }
        );
    }

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        const string sql = "SELECT id, name FROM roles WHERE id = @id";

        return await _dbHelper.QuerySingleOrDefaultAsync(
            sql,
            MapRole,
            new NpgsqlParameter("@id", NpgsqlDbType.Uuid) { Value = id }
        );
    }

    public async Task<List<Role>> GetAllAsync()
    {
        const string sql = "SELECT id, name FROM roles";

        return await _dbHelper.QueryAsync(sql, MapRole);
    }

    private static Role MapRole(NpgsqlDataReader reader)
    {
        return new Role
        {
            Id = reader.GetGuid(reader.GetOrdinal("id")),
            Name = reader.GetString(reader.GetOrdinal("name"))
        };
    }
}