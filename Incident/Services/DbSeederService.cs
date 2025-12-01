using Incident.Infrastructure;
using Npgsql;

namespace Incident.Services;

public class DbSeederService : IDbSeederService
{
    private readonly IDbHelper _dbHelper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<DbSeederService> _logger;

    public DbSeederService(
        IDbHelper dbHelper,
        IPasswordHasher passwordHasher,
        ILogger<DbSeederService> logger)
    {
        _dbHelper = dbHelper;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedUsersAsync();
    }

    private async Task SeedRolesAsync()
    {
        const string checkSql = "SELECT COUNT(1) FROM incident.roles";
        var count = await _dbHelper.ExecuteScalarAsync<long>(checkSql);

        if (count > 0)
        {
            _logger.LogInformation("Roles already seeded, skipping...");
            return;
        }

        _logger.LogInformation("Seeding roles...");

        var roles = new[]
        {
            ("secretary", "secretary"),
            ("officer", "officer"),
            ("supervisor", "supervisor")
        };

        foreach (var (id, name) in roles)
        {
            const string sql = @"
                INSERT INTO incident.roles (id, name)
                VALUES (@Id, @Name)
                ON CONFLICT (name) DO NOTHING";

            await _dbHelper.ExecuteAsync(sql,
                new NpgsqlParameter("@Id", Guid.NewGuid()),
                new NpgsqlParameter("@Name", name));
        }

        _logger.LogInformation("Roles seeded successfully");
    }

    private async Task SeedUsersAsync()
    {
        const string checkSql = "SELECT COUNT(1) FROM incident.users";
        var count = await _dbHelper.ExecuteScalarAsync<long>(checkSql);

        if (count > 0)
        {
            _logger.LogInformation("Users already seeded, skipping...");
            return;
        }

        _logger.LogInformation("Seeding users...");

        // Get role IDs
        var roles = new Dictionary<string, Guid>();
        const string getRolesSql = "SELECT id, name FROM incident.roles";
        var roleResults = await _dbHelper.QueryAsync(getRolesSql, reader => new
        {
            Id = reader.GetGuid(0),
            Name = reader.GetString(1)
        });

        foreach (var role in roleResults)
        {
            roles[role.Name] = role.Id;
        }

        // Seed default users
        var users = new[]
        {
            ("secretary1", "password123", "secretary"),
            ("officer1", "password123", "officer"),
            ("officer2", "password123", "officer"),
            ("supervisor1", "password123", "supervisor")
        };

        foreach (var (username, password, roleName) in users)
        {
            if (!roles.TryGetValue(roleName, out var roleId))
            {
                _logger.LogWarning("Role {RoleName} not found, skipping user {Username}", roleName, username);
                continue;
            }

            var (hash, salt) = _passwordHasher.HashPassword(password);

            const string sql = @"
                INSERT INTO incident.users (id, username, password_hash, password_salt, role_id, created_at, updated_at)
                VALUES (@Id, @Username, @PasswordHash, @PasswordSalt, @RoleId, @CreatedAt, @UpdatedAt)
                ON CONFLICT (username) DO NOTHING";

            await _dbHelper.ExecuteAsync(sql,
                new NpgsqlParameter("@Id", Guid.NewGuid()),
                new NpgsqlParameter("@Username", username),
                new NpgsqlParameter("@PasswordHash", hash),
                new NpgsqlParameter("@PasswordSalt", salt),
                new NpgsqlParameter("@RoleId", roleId),
                new NpgsqlParameter("@CreatedAt", DateTime.UtcNow),
                new NpgsqlParameter("@UpdatedAt", DateTime.UtcNow));
        }

        _logger.LogInformation("Users seeded successfully");
    }
}