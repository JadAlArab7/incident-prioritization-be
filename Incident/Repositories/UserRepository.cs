using Dapper;
using Incident.Infrastructure;
using Incident.Models;

namespace Incident.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbHelper _dbHelper;

    public UserRepository(IDbHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        using var connection = _dbHelper.CreateConnection();
        const string sql = @"
            SELECT 
                u.id, u.username, u.password_hash as PasswordHash, u.password_salt as PasswordSalt,
                u.role_id as RoleId, u.created_at as CreatedAt, u.updated_at as UpdatedAt,
                r.id, r.name
            FROM incident.users u
            LEFT JOIN incident.roles r ON u.role_id = r.id
            ORDER BY u.username";

        var users = await connection.QueryAsync<User, Role, User>(
            sql,
            (user, role) =>
            {
                user.Role = role;
                return user;
            },
            splitOn: "id"
        );

        return users;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        using var connection = _dbHelper.CreateConnection();
        const string sql = @"
            SELECT 
                u.id, u.username, u.password_hash as PasswordHash, u.password_salt as PasswordSalt,
                u.role_id as RoleId, u.created_at as CreatedAt, u.updated_at as UpdatedAt,
                r.id, r.name
            FROM incident.users u
            LEFT JOIN incident.roles r ON u.role_id = r.id
            WHERE u.id = @Id";

        var users = await connection.QueryAsync<User, Role, User>(
            sql,
            (user, role) =>
            {
                user.Role = role;
                return user;
            },
            new { Id = id },
            splitOn: "id"
        );

        return users.FirstOrDefault();
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = _dbHelper.CreateConnection();
        const string sql = @"
            SELECT 
                u.id, u.username, u.password_hash as PasswordHash, u.password_salt as PasswordSalt,
                u.role_id as RoleId, u.created_at as CreatedAt, u.updated_at as UpdatedAt,
                r.id, r.name
            FROM incident.users u
            LEFT JOIN incident.roles r ON u.role_id = r.id
            WHERE u.username = @Username";

        var users = await connection.QueryAsync<User, Role, User>(
            sql,
            (user, role) =>
            {
                user.Role = role;
                return user;
            },
            new { Username = username },
            splitOn: "id"
        );

        return users.FirstOrDefault();
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(string roleName)
    {
        using var connection = _dbHelper.CreateConnection();
        const string sql = @"
            SELECT 
                u.id, u.username, u.password_hash as PasswordHash, u.password_salt as PasswordSalt,
                u.role_id as RoleId, u.created_at as CreatedAt, u.updated_at as UpdatedAt,
                r.id, r.name
            FROM incident.users u
            LEFT JOIN incident.roles r ON u.role_id = r.id
            WHERE r.name = @RoleName
            ORDER BY u.username";

        var users = await connection.QueryAsync<User, Role, User>(
            sql,
            (user, role) =>
            {
                user.Role = role;
                return user;
            },
            new { RoleName = roleName },
            splitOn: "id"
        );

        return users;
    }

    public async Task<User?> CreateAsync(User user)
    {
        using var connection = _dbHelper.CreateConnection();
        const string sql = @"
            INSERT INTO incident.users (id, username, password_hash, password_salt, role_id, created_at, updated_at)
            VALUES (@Id, @Username, @PasswordHash, @PasswordSalt, @RoleId, @CreatedAt, @UpdatedAt)
            RETURNING id, username, password_hash as PasswordHash, password_salt as PasswordSalt, 
                      role_id as RoleId, created_at as CreatedAt, updated_at as UpdatedAt";

        return await connection.QuerySingleOrDefaultAsync<User>(sql, user);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using var connection = _dbHelper.CreateConnection();
        const string sql = "DELETE FROM incident.users WHERE id = @Id";

        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }
}