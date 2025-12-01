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

    public async Task<User?> GetByIdAsync(Guid id)
    {
        const string sql = @"
            SELECT u.id, u.username, u.email, u.password_hash as PasswordHash, 
                   u.full_name as FullName, u.role_id as RoleId, u.is_active as IsActive,
                   u.created_at as CreatedAt, u.updated_at as UpdatedAt,
                   r.id as RoleId, r.code as Code, r.name as Name, r.description as Description
            FROM users u
            LEFT JOIN roles r ON u.role_id = r.id
            WHERE u.id = @Id";

        var result = await _dbHelper.QueryAsync<User, Role, User>(
            sql,
            (user, role) =>
            {
                if (role?.Id != Guid.Empty)
                {
                    user.Role = role;
                }
                return user;
            },
            new { Id = id },
            splitOn: "RoleId"
        );

        return result.FirstOrDefault();
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        const string sql = @"
            SELECT u.id, u.username, u.email, u.password_hash as PasswordHash, 
                   u.full_name as FullName, u.role_id as RoleId, u.is_active as IsActive,
                   u.created_at as CreatedAt, u.updated_at as UpdatedAt,
                   r.id as RoleId, r.code as Code, r.name as Name, r.description as Description
            FROM users u
            LEFT JOIN roles r ON u.role_id = r.id
            WHERE u.username = @Username";

        var result = await _dbHelper.QueryAsync<User, Role, User>(
            sql,
            (user, role) =>
            {
                if (role?.Id != Guid.Empty)
                {
                    user.Role = role;
                }
                return user;
            },
            new { Username = username },
            splitOn: "RoleId"
        );

        return result.FirstOrDefault();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        const string sql = @"
            SELECT u.id, u.username, u.email, u.password_hash as PasswordHash, 
                   u.full_name as FullName, u.role_id as RoleId, u.is_active as IsActive,
                   u.created_at as CreatedAt, u.updated_at as UpdatedAt,
                   r.id as RoleId, r.code as Code, r.name as Name, r.description as Description
            FROM users u
            LEFT JOIN roles r ON u.role_id = r.id
            WHERE u.email = @Email";

        var result = await _dbHelper.QueryAsync<User, Role, User>(
            sql,
            (user, role) =>
            {
                if (role?.Id != Guid.Empty)
                {
                    user.Role = role;
                }
                return user;
            },
            new { Email = email },
            splitOn: "RoleId"
        );

        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        const string sql = @"
            SELECT u.id, u.username, u.email, u.password_hash as PasswordHash, 
                   u.full_name as FullName, u.role_id as RoleId, u.is_active as IsActive,
                   u.created_at as CreatedAt, u.updated_at as UpdatedAt,
                   r.id as RoleId, r.code as Code, r.name as Name, r.description as Description
            FROM users u
            LEFT JOIN roles r ON u.role_id = r.id
            ORDER BY u.created_at DESC";

        return await _dbHelper.QueryAsync<User, Role, User>(
            sql,
            (user, role) =>
            {
                if (role?.Id != Guid.Empty)
                {
                    user.Role = role;
                }
                return user;
            },
            splitOn: "RoleId"
        );
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(string roleCode)
    {
        const string sql = @"
            SELECT u.id, u.username, u.email, u.password_hash as PasswordHash, 
                   u.full_name as FullName, u.role_id as RoleId, u.is_active as IsActive,
                   u.created_at as CreatedAt, u.updated_at as UpdatedAt,
                   r.id as RoleId, r.code as Code, r.name as Name, r.description as Description
            FROM users u
            LEFT JOIN roles r ON u.role_id = r.id
            WHERE r.code = @RoleCode
            ORDER BY u.created_at DESC";

        return await _dbHelper.QueryAsync<User, Role, User>(
            sql,
            (user, role) =>
            {
                if (role?.Id != Guid.Empty)
                {
                    user.Role = role;
                }
                return user;
            },
            new { RoleCode = roleCode },
            splitOn: "RoleId"
        );
    }

    public async Task<User> CreateAsync(User user)
    {
        const string sql = @"
            INSERT INTO users (id, username, email, password_hash, full_name, role_id, is_active, created_at, updated_at)
            VALUES (@Id, @Username, @Email, @PasswordHash, @FullName, @RoleId, @IsActive, @CreatedAt, @UpdatedAt)
            RETURNING id, username, email, password_hash as PasswordHash, full_name as FullName, 
                      role_id as RoleId, is_active as IsActive, created_at as CreatedAt, updated_at as UpdatedAt";

        user.Id = Guid.NewGuid();
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        user.IsActive = true;

        var createdUser = await _dbHelper.QuerySingleOrDefaultAsync<User>(sql, new
        {
            user.Id,
            user.Username,
            user.Email,
            user.PasswordHash,
            user.FullName,
            user.RoleId,
            user.IsActive,
            user.CreatedAt,
            user.UpdatedAt
        });

        return createdUser ?? user;
    }

    public async Task<User?> UpdateAsync(User user)
    {
        const string sql = @"
            UPDATE users 
            SET username = @Username, 
                email = @Email, 
                full_name = @FullName, 
                role_id = @RoleId, 
                is_active = @IsActive, 
                updated_at = @UpdatedAt
            WHERE id = @Id
            RETURNING id, username, email, password_hash as PasswordHash, full_name as FullName, 
                      role_id as RoleId, is_active as IsActive, created_at as CreatedAt, updated_at as UpdatedAt";

        user.UpdatedAt = DateTime.UtcNow;

        return await _dbHelper.QuerySingleOrDefaultAsync<User>(sql, new
        {
            user.Id,
            user.Username,
            user.Email,
            user.FullName,
            user.RoleId,
            user.IsActive,
            user.UpdatedAt
        });
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string sql = "DELETE FROM users WHERE id = @Id";
        var rowsAffected = await _dbHelper.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        const string sql = "SELECT COUNT(1) FROM users WHERE id = @Id";
        var count = await _dbHelper.ExecuteScalarAsync<int>(sql, new { Id = id });
        return count > 0;
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        const string sql = "SELECT COUNT(1) FROM users WHERE username = @Username";
        var count = await _dbHelper.ExecuteScalarAsync<int>(sql, new { Username = username });
        return count > 0;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        const string sql = "SELECT COUNT(1) FROM users WHERE email = @Email";
        var count = await _dbHelper.ExecuteScalarAsync<int>(sql, new { Email = email });
        return count > 0;
    }
}