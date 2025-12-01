using Incident.Infrastructure;
using Incident.Models;

namespace Incident.Services;

public class DbSeederService : IDbSeederService
{
    private readonly IDbHelper _dbHelper;
    private readonly IPasswordHasher _passwordHasher;

    public DbSeederService(IDbHelper dbHelper, IPasswordHasher passwordHasher)
    {
        _dbHelper = dbHelper;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedUsersAsync();
        await SeedIncidentStatusesAsync();
        await SeedIncidentTypesAsync();
    }

    private async Task SeedRolesAsync()
    {
        const string checkSql = "SELECT COUNT(1) FROM roles";
        var count = await _dbHelper.ExecuteScalarAsync<int>(checkSql);

        if (count > 0) return;

        var roles = new[]
        {
            new { Id = Guid.NewGuid(), Code = "admin", Name = "Administrator", Description = "System administrator with full access" },
            new { Id = Guid.NewGuid(), Code = "officer", Name = "Officer", Description = "Officer who reviews and processes incidents" },
            new { Id = Guid.NewGuid(), Code = "user", Name = "User", Description = "Regular user who can create incidents" }
        };

        foreach (var role in roles)
        {
            const string sql = @"
                INSERT INTO roles (id, code, name, description)
                VALUES (@Id, @Code, @Name, @Description)
                ON CONFLICT (code) DO NOTHING";

            await _dbHelper.ExecuteAsync(sql, role);
        }
    }

    private async Task SeedUsersAsync()
    {
        const string checkSql = "SELECT COUNT(1) FROM users";
        var count = await _dbHelper.ExecuteScalarAsync<int>(checkSql);

        if (count > 0) return;

        // Get role IDs
        var adminRole = await _dbHelper.QuerySingleOrDefaultAsync<Role>(
            "SELECT id, code, name, description FROM roles WHERE code = @Code",
            new { Code = "admin" });

        var officerRole = await _dbHelper.QuerySingleOrDefaultAsync<Role>(
            "SELECT id, code, name, description FROM roles WHERE code = @Code",
            new { Code = "officer" });

        var userRole = await _dbHelper.QuerySingleOrDefaultAsync<Role>(
            "SELECT id, code, name, description FROM roles WHERE code = @Code",
            new { Code = "user" });

        if (adminRole == null || officerRole == null || userRole == null)
        {
            return;
        }

        var users = new[]
        {
            new
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                Email = "admin@example.com",
                PasswordHash = _passwordHasher.HashPassword("admin123"),
                FullName = "System Administrator",
                RoleId = adminRole.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new
            {
                Id = Guid.NewGuid(),
                Username = "officer1",
                Email = "officer1@example.com",
                PasswordHash = _passwordHasher.HashPassword("officer123"),
                FullName = "John Officer",
                RoleId = officerRole.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new
            {
                Id = Guid.NewGuid(),
                Username = "officer2",
                Email = "officer2@example.com",
                PasswordHash = _passwordHasher.HashPassword("officer123"),
                FullName = "Jane Officer",
                RoleId = officerRole.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new
            {
                Id = Guid.NewGuid(),
                Username = "user1",
                Email = "user1@example.com",
                PasswordHash = _passwordHasher.HashPassword("user123"),
                FullName = "Regular User",
                RoleId = userRole.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        foreach (var user in users)
        {
            const string sql = @"
                INSERT INTO users (id, username, email, password_hash, full_name, role_id, is_active, created_at, updated_at)
                VALUES (@Id, @Username, @Email, @PasswordHash, @FullName, @RoleId, @IsActive, @CreatedAt, @UpdatedAt)
                ON CONFLICT (username) DO NOTHING";

            await _dbHelper.ExecuteAsync(sql, user);
        }
    }

    private async Task SeedIncidentStatusesAsync()
    {
        const string checkSql = "SELECT COUNT(1) FROM incident_statuses";
        var count = await _dbHelper.ExecuteScalarAsync<int>(checkSql);

        if (count > 0) return;

        var statuses = new[]
        {
            new { Id = Guid.NewGuid(), Code = "draft", Name = "Draft", Description = "Incident is in draft state", IsTerminal = false },
            new { Id = Guid.NewGuid(), Code = "in_review", Name = "In Review", Description = "Incident is being reviewed", IsTerminal = false },
            new { Id = Guid.NewGuid(), Code = "accepted", Name = "Accepted", Description = "Incident has been accepted", IsTerminal = true },
            new { Id = Guid.NewGuid(), Code = "rejected", Name = "Rejected", Description = "Incident has been rejected", IsTerminal = false }
        };

        foreach (var status in statuses)
        {
            const string sql = @"
                INSERT INTO incident_statuses (id, code, name, description, is_terminal)
                VALUES (@Id, @Code, @Name, @Description, @IsTerminal)
                ON CONFLICT (code) DO NOTHING";

            await _dbHelper.ExecuteAsync(sql, status);
        }

        // Seed transitions after statuses are created
        await SeedIncidentTransitionsAsync();
    }

    private async Task SeedIncidentTransitionsAsync()
    {
        const string checkSql = "SELECT COUNT(1) FROM incident_status_transitions";
        var count = await _dbHelper.ExecuteScalarAsync<int>(checkSql);

        if (count > 0) return;

        // Get status IDs
        var draftStatus = await _dbHelper.QuerySingleOrDefaultAsync<IncidentStatus>(
            "SELECT id, code, name, description, is_terminal as IsTerminal FROM incident_statuses WHERE code = @Code",
            new { Code = "draft" });

        var inReviewStatus = await _dbHelper.QuerySingleOrDefaultAsync<IncidentStatus>(
            "SELECT id, code, name, description, is_terminal as IsTerminal FROM incident_statuses WHERE code = @Code",
            new { Code = "in_review" });

        var acceptedStatus = await _dbHelper.QuerySingleOrDefaultAsync<IncidentStatus>(
            "SELECT id, code, name, description, is_terminal as IsTerminal FROM incident_statuses WHERE code = @Code",
            new { Code = "accepted" });

        var rejectedStatus = await _dbHelper.QuerySingleOrDefaultAsync<IncidentStatus>(
            "SELECT id, code, name, description, is_terminal as IsTerminal FROM incident_statuses WHERE code = @Code",
            new { Code = "rejected" });

        if (draftStatus == null || inReviewStatus == null || acceptedStatus == null || rejectedStatus == null)
        {
            return;
        }

        var transitions = new[]
        {
            new { Id = Guid.NewGuid(), FromStatusId = draftStatus.Id, ToStatusId = inReviewStatus.Id, ActionCode = "send_to_review", Initiator = "creator", IsActive = true },
            new { Id = Guid.NewGuid(), FromStatusId = rejectedStatus.Id, ToStatusId = inReviewStatus.Id, ActionCode = "send_to_review", Initiator = "creator", IsActive = true },
            new { Id = Guid.NewGuid(), FromStatusId = inReviewStatus.Id, ToStatusId = acceptedStatus.Id, ActionCode = "accept", Initiator = "officer", IsActive = true },
            new { Id = Guid.NewGuid(), FromStatusId = inReviewStatus.Id, ToStatusId = rejectedStatus.Id, ActionCode = "reject", Initiator = "officer", IsActive = true }
        };

        foreach (var transition in transitions)
        {
            const string sql = @"
                INSERT INTO incident_status_transitions (id, from_status_id, to_status_id, action_code, initiator, is_active)
                VALUES (@Id, @FromStatusId, @ToStatusId, @ActionCode, @Initiator, @IsActive)";

            await _dbHelper.ExecuteAsync(sql, transition);
        }
    }

    private async Task SeedIncidentTypesAsync()
    {
        const string checkSql = "SELECT COUNT(1) FROM incident_types";
        var count = await _dbHelper.ExecuteScalarAsync<int>(checkSql);

        if (count > 0) return;

        var types = new[]
        {
            new { Id = Guid.NewGuid(), Code = "fire", Name = "Fire", Description = "Fire-related incidents" },
            new { Id = Guid.NewGuid(), Code = "flood", Name = "Flood", Description = "Flood-related incidents" },
            new { Id = Guid.NewGuid(), Code = "accident", Name = "Accident", Description = "Traffic or other accidents" },
            new { Id = Guid.NewGuid(), Code = "medical", Name = "Medical Emergency", Description = "Medical emergency incidents" },
            new { Id = Guid.NewGuid(), Code = "crime", Name = "Crime", Description = "Criminal activity incidents" },
            new { Id = Guid.NewGuid(), Code = "infrastructure", Name = "Infrastructure", Description = "Infrastructure damage or issues" },
            new { Id = Guid.NewGuid(), Code = "other", Name = "Other", Description = "Other types of incidents" }
        };

        foreach (var type in types)
        {
            const string sql = @"
                INSERT INTO incident_types (id, code, name, description)
                VALUES (@Id, @Code, @Name, @Description)
                ON CONFLICT (code) DO NOTHING";

            await _dbHelper.ExecuteAsync(sql, type);
        }
    }
}