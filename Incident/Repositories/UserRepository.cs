using Incident.Infrastructure;
using Incident.Models;
using Npgsql;

namespace Incident.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbHelper _dbHelper;

    public UserRepository(IDbHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT u.id, u.username, u.email, u.password_hash, u.full_name, 
                   u.role_id, r.name as role_name, r.code as role_code,
                   u.is_active, u.created_at, u.updated_at
            FROM incident.users u
            LEFT JOIN incident.roles r ON u.role_id = r.id
            WHERE u.id = @id";

        var parameters = new[] { new NpgsqlParameter("@id", id) };

        return await _dbHelper.ExecuteReaderSingleAsync(sql, MapUser, ct, parameters);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT u.id, u.username, u.email, u.password_hash, u.full_name, 
                   u.role_id, r.name as role_name, r.code as role_code,
                   u.is_active, u.created_at, u.updated_at
            FROM incident.users u
            LEFT JOIN incident.roles r ON u.role_id = r.id
            WHERE u.username = @username";

        var parameters = new[] { new NpgsqlParameter("@username", username) };

        return await _dbHelper.ExecuteReaderSingleAsync(sql, MapUser, ct, parameters);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT u.id, u.username, u.email, u.password_hash, u.full_name, 
                   u.role_id, r.name as role_name, r.code as role_code,
                   u.is_active, u.created_at, u.updated_at
            FROM incident.users u
            LEFT JOIN incident.roles r ON u.role_id = r.id
            WHERE u.email = @email";

        var parameters = new[] { new NpgsqlParameter("@email", email) };

        return await _dbHelper.ExecuteReaderSingleAsync(sql, MapUser, ct, parameters);
    }

    public async Task<List<User>> GetAllAsync(CancellationToken ct = default)
    {
        const string sql = @"
            SELECT u.id, u.username, u.email, u.password_hash, u.full_name, 
                   u.role_id, r.name as role_name, r.code as role_code,
                   u.is_active, u.created_at, u.updated_at
            FROM incident.users u
            LEFT JOIN incident.roles r ON u.role_id = r.id
            ORDER BY u.created_at DESC";

        return await _dbHelper.ExecuteReaderAsync(sql, MapUser, ct);
    }

    public async Task<Guid> CreateAsync(User user, CancellationToken ct = default)
    {
        const string sql = @"
            INSERT INTO incident.users (id, username, email, password_hash, full_name, role_id, is_active, created_at, updated_at)
            VALUES (@id, @username, @email, @passwordHash, @fullName, @roleId, @isActive, @createdAt, @updatedAt)
            RETURNING id";

        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var parameters = new[]
        {
            new NpgsqlParameter("@id", id),
            new NpgsqlParameter("@username", user.Username),
            new NpgsqlParameter("@email", user.Email),
            new NpgsqlParameter("@passwordHash", user.PasswordHash),
            new NpgsqlParameter("@fullName", (object?)user.FullName ?? DBNull.Value),
            new NpgsqlParameter("@roleId", user.RoleId),
            new NpgsqlParameter("@isActive", user.IsActive),
            new NpgsqlParameter("@createdAt", now),
            new NpgsqlParameter("@updatedAt", now)
        };

        var result = await _dbHelper.ExecuteScalarAsync<Guid>(sql, ct, parameters);
        return result;
    }

    public async Task<bool> UpdateAsync(User user, CancellationToken ct = default)
    {
        const string sql = @"
            UPDATE incident.users 
            SET username = @username, email = @email, full_name = @fullName, 
                role_id = @roleId, is_active = @isActive, updated_at = @updatedAt
            WHERE id = @id";

        var parameters = new[]
        {
            new NpgsqlParameter("@id", user.Id),
            new NpgsqlParameter("@username", user.Username),
            new NpgsqlParameter("@email", user.Email),
            new NpgsqlParameter("@fullName", (object?)user.FullName ?? DBNull.Value),
            new NpgsqlParameter("@roleId", user.RoleId),
            new NpgsqlParameter("@isActive", user.IsActive),
            new NpgsqlParameter("@updatedAt", DateTime.UtcNow)
        };

        var rowsAffected = await _dbHelper.ExecuteNonQueryAsync(sql, ct, parameters);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = "DELETE FROM incident.users WHERE id = @id";
        var parameters = new[] { new NpgsqlParameter("@id", id) };

        var rowsAffected = await _dbHelper.ExecuteNonQueryAsync(sql, ct, parameters);
        return rowsAffected > 0;
    }

    private static User MapUser(NpgsqlDataReader reader)
    {
        return new User
        {
            Id = reader.GetGuid(0),
            Username = reader.GetString(1),
            Email = reader.GetString(2),
            PasswordHash = reader.GetString(3),
            FullName = reader.IsDBNull(4) ? null : reader.GetString(4),
            RoleId = reader.GetGuid(5),
            RoleName = reader.IsDBNull(6) ? null : reader.GetString(6),
            RoleCode = reader.IsDBNull(7) ? null : reader.GetString(7),
            IsActive = reader.GetBoolean(8),
            CreatedAt = reader.GetDateTime(9),
            UpdatedAt = reader.GetDateTime(10)
        };
    }
}