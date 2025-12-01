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

    public async Task SeedAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Starting database seeding...");

        await SeedRolesAsync(ct);
        await SeedUsersAsync(ct);
        await SeedIncidentStatusesAsync(ct);
        await SeedIncidentTypesAsync(ct);
        await SeedStatusTransitionsAsync(ct);

        _logger.LogInformation("Database seeding completed.");
    }

    public async Task SeedRolesAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Seeding roles...");

        const string sql = @"
            INSERT INTO incident.roles (id, code, name)
            VALUES 
                (@adminId, 'admin', 'Administrator'),
                (@officerId, 'officer', 'Officer'),
                (@userId, 'user', 'User')
            ON CONFLICT (id) DO NOTHING";

        var parameters = new[]
        {
            new NpgsqlParameter("@adminId", Guid.Parse("00000000-0000-0000-0000-000000000001")),
            new NpgsqlParameter("@officerId", Guid.Parse("00000000-0000-0000-0000-000000000002")),
            new NpgsqlParameter("@userId", Guid.Parse("00000000-0000-0000-0000-000000000003"))
        };

        await _dbHelper.ExecuteNonQueryAsync(sql, ct, parameters);
        _logger.LogInformation("Roles seeded successfully.");
    }

    public async Task SeedUsersAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Seeding users...");

        // Check if admin user already exists
        const string checkSql = "SELECT COUNT(*) FROM incident.users WHERE username = @username";
        var checkParams = new[] { new NpgsqlParameter("@username", "admin") };
        var count = await _dbHelper.ExecuteScalarAsync<long>(checkSql, ct, checkParams);

        if (count > 0)
        {
            _logger.LogInformation("Users already seeded, skipping...");
            return;
        }

        var adminPasswordHash = _passwordHasher.HashPassword("admin123");
        var officerPasswordHash = _passwordHasher.HashPassword("officer123");
        var userPasswordHash = _passwordHasher.HashPassword("user123");

        const string sql = @"
            INSERT INTO incident.users (id, username, email, password_hash, full_name, role_id, is_active, created_at, updated_at)
            VALUES 
                (@adminId, 'admin', 'admin@example.com', @adminPassword, 'System Administrator', @adminRoleId, true, NOW(), NOW()),
                (@officerId, 'officer', 'officer@example.com', @officerPassword, 'Default Officer', @officerRoleId, true, NOW(), NOW()),
                (@userId, 'user', 'user@example.com', @userPassword, 'Default User', @userRoleId, true, NOW(), NOW())
            ON CONFLICT (id) DO NOTHING";

        var parameters = new[]
        {
            new NpgsqlParameter("@adminId", Guid.Parse("10000000-0000-0000-0000-000000000001")),
            new NpgsqlParameter("@officerId", Guid.Parse("10000000-0000-0000-0000-000000000002")),
            new NpgsqlParameter("@userId", Guid.Parse("10000000-0000-0000-0000-000000000003")),
            new NpgsqlParameter("@adminPassword", adminPasswordHash),
            new NpgsqlParameter("@officerPassword", officerPasswordHash),
            new NpgsqlParameter("@userPassword", userPasswordHash),
            new NpgsqlParameter("@adminRoleId", Guid.Parse("00000000-0000-0000-0000-000000000001")),
            new NpgsqlParameter("@officerRoleId", Guid.Parse("00000000-0000-0000-0000-000000000002")),
            new NpgsqlParameter("@userRoleId", Guid.Parse("00000000-0000-0000-0000-000000000003"))
        };

        await _dbHelper.ExecuteNonQueryAsync(sql, ct, parameters);
        _logger.LogInformation("Users seeded successfully.");
    }

    public async Task SeedIncidentStatusesAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Seeding incident statuses...");

        const string sql = @"
            INSERT INTO incident.incident_statuses (id, code, name)
            VALUES 
                (@draftId, 'draft', 'Draft'),
                (@inReviewId, 'in_review', 'In Review'),
                (@acceptedId, 'accepted', 'Accepted'),
                (@rejectedId, 'rejected', 'Rejected')
            ON CONFLICT (id) DO NOTHING";

        var parameters = new[]
        {
            new NpgsqlParameter("@draftId", Guid.Parse("00000000-0000-0000-0000-00000000d001")),
            new NpgsqlParameter("@inReviewId", Guid.Parse("00000000-0000-0000-0000-00000000r002")),
            new NpgsqlParameter("@acceptedId", Guid.Parse("00000000-0000-0000-0000-00000000a003")),
            new NpgsqlParameter("@rejectedId", Guid.Parse("00000000-0000-0000-0000-00000000j004"))
        };

        await _dbHelper.ExecuteNonQueryAsync(sql, ct, parameters);
        _logger.LogInformation("Incident statuses seeded successfully.");
    }

    public async Task SeedIncidentTypesAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Seeding incident types...");

        const string sql = @"
            INSERT INTO incident.incident_types (id, code, name)
            VALUES 
                (@fireId, 'fire', 'Fire'),
                (@floodId, 'flood', 'Flood'),
                (@accidentId, 'accident', 'Accident'),
                (@crimeId, 'crime', 'Crime'),
                (@medicalId, 'medical', 'Medical Emergency'),
                (@otherId, 'other', 'Other')
            ON CONFLICT (id) DO NOTHING";

        var parameters = new[]
        {
            new NpgsqlParameter("@fireId", Guid.Parse("20000000-0000-0000-0000-000000000001")),
            new NpgsqlParameter("@floodId", Guid.Parse("20000000-0000-0000-0000-000000000002")),
            new NpgsqlParameter("@accidentId", Guid.Parse("20000000-0000-0000-0000-000000000003")),
            new NpgsqlParameter("@crimeId", Guid.Parse("20000000-0000-0000-0000-000000000004")),
            new NpgsqlParameter("@medicalId", Guid.Parse("20000000-0000-0000-0000-000000000005")),
            new NpgsqlParameter("@otherId", Guid.Parse("20000000-0000-0000-0000-000000000006"))
        };

        await _dbHelper.ExecuteNonQueryAsync(sql, ct, parameters);
        _logger.LogInformation("Incident types seeded successfully.");
    }

    public async Task SeedStatusTransitionsAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Seeding status transitions...");

        const string sql = @"
            INSERT INTO incident.incident_status_transitions (id, from_status_id, to_status_id, action_code, initiator, is_active)
            VALUES 
                (@t1Id, @draftId, @inReviewId, 'send_to_review', 'creator', true),
                (@t2Id, @inReviewId, @acceptedId, 'accept', 'officer', true),
                (@t3Id, @inReviewId, @rejectedId, 'reject', 'officer', true),
                (@t4Id, @rejectedId, @inReviewId, 'send_to_review', 'creator', true)
            ON CONFLICT (id) DO NOTHING";

        var parameters = new[]
        {
            new NpgsqlParameter("@t1Id", Guid.Parse("30000000-0000-0000-0000-000000000001")),
            new NpgsqlParameter("@t2Id", Guid.Parse("30000000-0000-0000-0000-000000000002")),
            new NpgsqlParameter("@t3Id", Guid.Parse("30000000-0000-0000-0000-000000000003")),
            new NpgsqlParameter("@t4Id", Guid.Parse("30000000-0000-0000-0000-000000000004")),
            new NpgsqlParameter("@draftId", Guid.Parse("00000000-0000-0000-0000-00000000d001")),
            new NpgsqlParameter("@inReviewId", Guid.Parse("00000000-0000-0000-0000-00000000r002")),
            new NpgsqlParameter("@acceptedId", Guid.Parse("00000000-0000-0000-0000-00000000a003")),
            new NpgsqlParameter("@rejectedId", Guid.Parse("00000000-0000-0000-0000-00000000j004"))
        };

        await _dbHelper.ExecuteNonQueryAsync(sql, ct, parameters);
        _logger.LogInformation("Status transitions seeded successfully.");
    }
}