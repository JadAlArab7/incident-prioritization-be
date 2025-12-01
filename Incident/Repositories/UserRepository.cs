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

    public async Task<User?> GetByIdAsync(Guid id)
    {
        const string sql = @"
            SELECT u.id, u.username, u.password_hash, u.password_salt, 
                   u.role_id, u.created_at, u.updated_at, r.name as role_name
            FROM incident.users u
            JOIN incident.roles r ON u.role_id = r.id
            WHERE u.id = @Id";

        return await _dbHelper.QuerySingleOrDefaultAsync(sql, reader => new User
        {
            Id = reader.GetGuid(0),
            Username = reader.GetString(1),
            PasswordHash = (byte[])reader[2],
            PasswordSalt = (byte[])reader[3],
            RoleId = reader.GetGuid(4),
            CreatedAt = reader.GetDateTime(5),
            UpdatedAt = reader.GetDateTime(6),
            RoleName = reader.GetString(7)
        }, new NpgsqlParameter("@Id", id));
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        const string sql = @"
            SELECT u.id, u.username, u.password_hash, u.password_salt, 
                   u.role_id, u.created_at, u.updated_at, r.name as role_name
            FROM incident.users u
            JOIN incident.roles r ON u.role_id = r.id
            WHERE u.username = @Username";

        return await _dbHelper.QuerySingleOrDefaultAsync(sql, reader => new User
        {
            Id = reader.GetGuid(0),
            Username = reader.GetString(1),
            PasswordHash = (byte[])reader[2],
            PasswordSalt = (byte[])reader[3],
            RoleId = reader.GetGuid(4),
            CreatedAt = reader.GetDateTime(5),
            UpdatedAt = reader.GetDateTime(6),
            RoleName = reader.GetString(7)
        }, new NpgsqlParameter("@Username", username));
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        const string sql = @"
            SELECT u.id, u.username, u.password_hash, u.password_salt, 
                   u.role_id, u.created_at, u.updated_at, r.name as role_name
            FROM incident.users u
            JOIN incident.roles r ON u.role_id = r.id
            ORDER BY u.username";

        return await _dbHelper.QueryAsync(sql, reader => new User
        {
            Id = reader.GetGuid(0),
            Username = reader.GetString(1),
            PasswordHash = (byte[])reader[2],
            PasswordSalt = (byte[])reader[3],
            RoleId = reader.GetGuid(4),
            CreatedAt = reader.GetDateTime(5),
            UpdatedAt = reader.GetDateTime(6),
            RoleName = reader.GetString(7)
        });
    }

    public async Task<IEnumerable<User>> GetByRoleNameAsync(string roleName)
    {
        const string sql = @"
            SELECT u.id, u.username, u.password_hash, u.password_salt, 
                   u.role_id, u.created_at, u.updated_at, r.name as role_name
            FROM incident.users u
            JOIN incident.roles r ON u.role_id = r.id
            WHERE r.name = @RoleName
            ORDER BY u.username";

        return await _dbHelper.QueryAsync(sql, reader => new User
        {
            Id = reader.GetGuid(0),
            Username = reader.GetString(1),
            PasswordHash = (byte[])reader[2],
            PasswordSalt = (byte[])reader[3],
            RoleId = reader.GetGuid(4),
            CreatedAt = reader.GetDateTime(5),
            UpdatedAt = reader.GetDateTime(6),
            RoleName = reader.GetString(7)
        }, new NpgsqlParameter("@RoleName", roleName));
    }

    public async Task<User> CreateAsync(User user)
    {
        const string sql = @"
            INSERT INTO incident.users (id, username, password_hash, password_salt, role_id, created_at, updated_at)
            VALUES (@Id, @Username, @PasswordHash, @PasswordSalt, @RoleId, @CreatedAt, @UpdatedAt)
            RETURNING id, username, password_hash, password_salt, role_id, created_at, updated_at";

        var result = await _dbHelper.QuerySingleOrDefaultAsync(sql, reader => new User
        {
            Id = reader.GetGuid(0),
            Username = reader.GetString(1),
            PasswordHash = (byte[])reader[2],
            PasswordSalt = (byte[])reader[3],
            RoleId = reader.GetGuid(4),
            CreatedAt = reader.GetDateTime(5),
            UpdatedAt = reader.GetDateTime(6)
        },
        new NpgsqlParameter("@Id", user.Id),
        new NpgsqlParameter("@Username", user.Username),
        new NpgsqlParameter("@PasswordHash", user.PasswordHash),
        new NpgsqlParameter("@PasswordSalt", user.PasswordSalt),
        new NpgsqlParameter("@RoleId", user.RoleId),
        new NpgsqlParameter("@CreatedAt", user.CreatedAt),
        new NpgsqlParameter("@UpdatedAt", user.UpdatedAt));

        return result!;
    }

    public async Task<User> UpdateAsync(User user)
    {
        const string sql = @"
            UPDATE incident.users 
            SET username = @Username, 
                password_hash = @PasswordHash, 
                password_salt = @PasswordSalt, 
                role_id = @RoleId, 
                updated_at = @UpdatedAt
            WHERE id = @Id
            RETURNING id, username, password_hash, password_salt, role_id, created_at, updated_at";

        var result = await _dbHelper.QuerySingleOrDefaultAsync(sql, reader => new User
        {
            Id = reader.GetGuid(0),
            Username = reader.GetString(1),
            PasswordHash = (byte[])reader[2],
            PasswordSalt = (byte[])reader[3],
            RoleId = reader.GetGuid(4),
            CreatedAt = reader.GetDateTime(5),
            UpdatedAt = reader.GetDateTime(6)
        },
        new NpgsqlParameter("@Id", user.Id),
        new NpgsqlParameter("@Username", user.Username),
        new NpgsqlParameter("@PasswordHash", user.PasswordHash),
        new NpgsqlParameter("@PasswordSalt", user.PasswordSalt),
        new NpgsqlParameter("@RoleId", user.RoleId),
        new NpgsqlParameter("@UpdatedAt", DateTime.UtcNow));

        return result!;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string sql = "DELETE FROM incident.users WHERE id = @Id";
        var rowsAffected = await _dbHelper.ExecuteAsync(sql, new NpgsqlParameter("@Id", id));
        return rowsAffected > 0;
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        const string sql = "SELECT COUNT(1) FROM incident.users WHERE username = @Username";
        var count = await _dbHelper.ExecuteScalarAsync<long>(sql, new NpgsqlParameter("@Username", username));
        return count > 0;
    }
}