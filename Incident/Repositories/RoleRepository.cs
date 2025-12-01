using Incident.Infrastructure;
using Incident.Models;
using Npgsql;

namespace Incident.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly IDbHelper _db;

    public RoleRepository(IDbHelper db)
    {
        _db = db;
    }

    public async Task<Role?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = "SELECT id, name FROM incident.roles WHERE id = @id";

        await using var reader = await _db.ExecuteReaderAsync(sql, ct, new NpgsqlParameter("@id", id));

        if (await reader.ReadAsync(ct))
        {
            return new Role
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1)
            };
        }

        return null;
    }

    public async Task<Role?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        const string sql = "SELECT id, name FROM incident.roles WHERE name = @name";

        await using var reader = await _db.ExecuteReaderAsync(sql, ct, new NpgsqlParameter("@name", name));

        if (await reader.ReadAsync(ct))
        {
            return new Role
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1)
            };
        }

        return null;
    }

    public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken ct = default)
    {
        const string sql = "SELECT id, name FROM incident.roles ORDER BY name";

        var roles = new List<Role>();
        await using var reader = await _db.ExecuteReaderAsync(sql, ct);

        while (await reader.ReadAsync(ct))
        {
            roles.Add(new Role
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1)
            });
        }

        return roles;
    }
}