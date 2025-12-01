using Incident.Infrastructure;
using Incident.Models;
using Npgsql;
using NpgsqlTypes;

namespace Incident.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbHelper _dbHelper;

    public UserRepository(IDbHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        const string sql = @"
            SELECT u.id, u.username, u.password_hash, u.password_salt, u.role_id, r.name as role_name, u.created_at, u.updated_at
            FROM users u
            INNER JOIN roles r ON u.role_id = r.id
            WHERE u.username = @username";

        return await _dbHelper.QuerySingleOrDefaultAsync(
            sql,
            MapUser,
            new NpgsqlParameter("@username", NpgsqlDbType.Text) { Value = username }
        );
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        const string sql = @"
            SELECT u.id, u.username, u.password_hash, u.password_salt, u.role_id, r.name as role_name, u.created_at, u.updated_at
            FROM users u
            INNER JOIN roles r ON u.role_id = r.id
            WHERE u.id = @id";

        return await _dbHelper.QuerySingleOrDefaultAsync(
            sql,
            MapUser,
            new NpgsqlParameter("@id", NpgsqlDbType.Uuid) { Value = id }
        );
    }

    public async Task<Guid> CreateUserAsync(string username, byte[] passwordHash, byte[] passwordSalt, Guid roleId)
    {
        const string sql = @"
            INSERT INTO users (username, password_hash, password_salt, role_id)
            VALUES (@username, @password_hash, @password_salt, @role_id)
            RETURNING id";

        var result = await _dbHelper.ExecuteScalarAsync<Guid>(
            sql,
            new NpgsqlParameter("@username", NpgsqlDbType.Text) { Value = username },
            new NpgsqlParameter("@password_hash", NpgsqlDbType.Bytea) { Value = passwordHash },
            new NpgsqlParameter("@password_salt", NpgsqlDbType.Bytea) { Value = passwordSalt },
            new NpgsqlParameter("@role_id", NpgsqlDbType.Uuid) { Value = roleId }
        );

        return result;
    }

    public async Task<bool> UserExistsAsync(string username)
    {
        const string sql = "SELECT EXISTS(SELECT 1 FROM users WHERE username = @username)";

        var result = await _dbHelper.ExecuteScalarAsync<bool>(
            sql,
            new NpgsqlParameter("@username", NpgsqlDbType.Text) { Value = username }
        );

        return result;
    }

    private static User MapUser(NpgsqlDataReader reader)
    {
        return new User
        {
            Id = reader.GetGuid(reader.GetOrdinal("id")),
            Username = reader.GetString(reader.GetOrdinal("username")),
            PasswordHash = (byte[])reader["password_hash"],
            PasswordSalt = (byte[])reader["password_salt"],
            RoleId = reader.GetGuid(reader.GetOrdinal("role_id")),
            RoleName = reader.GetString(reader.GetOrdinal("role_name")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
        };
    }
}