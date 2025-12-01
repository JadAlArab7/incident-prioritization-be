using Incident.Infrastructure;
using Incident.Models;

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
            SELECT id, code, name, description
            FROM roles
            WHERE id = @Id";

        return await _dbHelper.QuerySingleOrDefaultAsync<Role>(sql, new { Id = id });
    }

    public async Task<Role?> GetByCodeAsync(string code)
    {
        const string sql = @"
            SELECT id, code, name, description
            FROM roles
            WHERE code = @Code";

        return await _dbHelper.QuerySingleOrDefaultAsync<Role>(sql, new { Code = code });
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        const string sql = @"
            SELECT id, code, name, description
            FROM roles
            ORDER BY name";

        return await _dbHelper.QueryAsync<Role>(sql);
    }

    public async Task<Role> CreateAsync(Role role)
    {
        const string sql = @"
            INSERT INTO roles (id, code, name, description)
            VALUES (@Id, @Code, @Name, @Description)
            RETURNING id, code, name, description";

        role.Id = Guid.NewGuid();

        var createdRole = await _dbHelper.QuerySingleOrDefaultAsync<Role>(sql, new
        {
            role.Id,
            role.Code,
            role.Name,
            role.Description
        });

        return createdRole ?? role;
    }

    public async Task<Role?> UpdateAsync(Role role)
    {
        const string sql = @"
            UPDATE roles 
            SET code = @Code, name = @Name, description = @Description
            WHERE id = @Id
            RETURNING id, code, name, description";

        return await _dbHelper.QuerySingleOrDefaultAsync<Role>(sql, new
        {
            role.Id,
            role.Code,
            role.Name,
            role.Description
        });
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string sql = "DELETE FROM roles WHERE id = @Id";
        var rowsAffected = await _dbHelper.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        const string sql = "SELECT COUNT(1) FROM roles WHERE id = @Id";
        var count = await _dbHelper.ExecuteScalarAsync<int>(sql, new { Id = id });
        return count > 0;
    }

    public async Task<bool> CodeExistsAsync(string code)
    {
        const string sql = "SELECT COUNT(1) FROM roles WHERE code = @Code";
        var count = await _dbHelper.ExecuteScalarAsync<int>(sql, new { Code = code });
        return count > 0;
    }
}