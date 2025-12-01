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

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        const string sql = @"
            SELECT id, name
            FROM incident.roles
            WHERE id = @Id";

        return await _dbHelper.QuerySingleOrDefaultAsync(sql, reader => new Role
        {
            Id = reader.GetGuid(0),
            Name = reader.GetString(1)
        }, new NpgsqlParameter("@Id", id));
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        const string sql = @"
            SELECT id, name
            FROM incident.roles
            WHERE name = @Name";

        return await _dbHelper.QuerySingleOrDefaultAsync(sql, reader => new Role
        {
            Id = reader.GetGuid(0),
            Name = reader.GetString(1)
        }, new NpgsqlParameter("@Name", name));
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        const string sql = @"
            SELECT id, name
            FROM incident.roles
            ORDER BY name";

        return await _dbHelper.QueryAsync(sql, reader => new Role
        {
            Id = reader.GetGuid(0),
            Name = reader.GetString(1)
        });
    }
}