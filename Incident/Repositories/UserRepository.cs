using Incident.Infrastructure;
using Incident.Models;
using Npgsql;

namespace Incident.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbHelper _db;

    public UserRepository(IDbHelper db)
    {
        _db = db;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT u.id, u.username, u.password_hash, u.password_salt, u.role_id, 
                   u.created_at, u.updated_at, r.name as role_name
            FROM incident.users u
            JOIN incident.roles r ON u.role_id = r.id
            WHERE u.id = @id";

        await using var reader = await _db.ExecuteReaderAsync(sql, ct, new NpgsqlParameter("@id", id));

        if (await reader.ReadAsync(ct))
        {
            return MapUserFromReader(reader);
        }

        return null;
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT u.id, u.username, u.password_hash, u.password_salt, u.role_id, 
                   u.created_at, u.updated_at, r.name as role_name
            FROM incident.users u
            JOIN incident.roles r ON u.role_id = r.id
            WHERE u.username = @username";

        await using var reader = await _db.ExecuteReaderAsync(sql, ct, new NpgsqlParameter("@username", username));

        if (await reader.ReadAsync(ct))
        {
            return MapUserFromReader(reader);
        }

        return null;
    }

    public async Task<Guid> CreateAsync(User user, CancellationToken ct = default)
    {
        const string sql = @"
            INSERT INTO incident.users (id, username, password_hash, password_salt, role_id)
            VALUES (@id, @username, @passwordHash, @passwordSalt, @roleId)
            RETURNING id";

        var id = Guid.NewGuid();
        await _db.ExecuteScalarAsync<Guid>(sql, ct,
            new NpgsqlParameter("@id", id),
            new NpgsqlParameter("@username", user.Username),
            new NpgsqlParameter("@passwordHash", user.PasswordHash),
            new NpgsqlParameter("@passwordSalt", user.PasswordSalt),
            new NpgsqlParameter("@roleId", user.RoleId));

        return id;
    }

    public async Task<bool> UpdateAsync(User user, CancellationToken ct = default)
    {
        const string sql = @"
            UPDATE incident.users
            SET username = @username, password_hash = @passwordHash, 
                password_salt = @passwordSalt, role_id = @roleId, updated_at = NOW()
            WHERE id = @id";

        var rows = await _db.ExecuteNonQueryAsync(sql, ct,
            new NpgsqlParameter("@id", user.Id),
            new NpgsqlParameter("@username", user.Username),
            new NpgsqlParameter("@passwordHash", user.PasswordHash),
            new NpgsqlParameter("@passwordSalt", user.PasswordSalt),
            new NpgsqlParameter("@roleId", user.RoleId));

        return rows > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = "DELETE FROM incident.users WHERE id = @id";
        var rows = await _db.ExecuteNonQueryAsync(sql, ct, new NpgsqlParameter("@id", id));
        return rows > 0;
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default)
    {
        const string sql = @"
            SELECT u.id, u.username, u.password_hash, u.password_salt, u.role_id, 
                   u.created_at, u.updated_at, r.name as role_name
            FROM incident.users u
            JOIN incident.roles r ON u.role_id = r.id
            ORDER BY u.username";

        var users = new List<User>();
        await using var reader = await _db.ExecuteReaderAsync(sql, ct);

        while (await reader.ReadAsync(ct))
        {
            users.Add(MapUserFromReader(reader));
        }

        return users;
    }

    private static User MapUserFromReader(NpgsqlDataReader reader)
    {
        return new User
        {
            Id = reader.GetGuid(0),
            Username = reader.GetString(1),
            PasswordHash = reader.IsDBNull(2) ? Array.Empty<byte>() : (byte[])reader[2],
            PasswordSalt = reader.IsDBNull(3) ? Array.Empty<byte>() : (byte[])reader[3],
            RoleId = reader.GetGuid(4),
            CreatedAt = reader.GetDateTime(5),
            UpdatedAt = reader.GetDateTime(6),
            Role = new Role
            {
                Id = reader.GetGuid(4),
                Name = reader.GetString(7)
            }
        };
    }
}