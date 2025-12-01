using Incident.Infrastructure;
using Incident.Models;
using Incident.Repositories;

namespace Incident.Services;

public class DbSeederService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;

    public DbSeederService(IRoleRepository roleRepository, IUserRepository userRepository)
    {
        _roleRepository = roleRepository;
        _userRepository = userRepository;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        // Check if admin user exists
        var adminUser = await _userRepository.GetByUsernameAsync("admin", ct);
        if (adminUser != null)
            return; // Already seeded

        // Get supervisor role
        var supervisorRole = await _roleRepository.GetByNameAsync("supervisor", ct);
        if (supervisorRole == null)
            throw new InvalidOperationException("Supervisor role not found. Please run the SQL seed scripts first.");

        // Create admin user
        var (hash, salt) = PasswordHasher.HashPassword("admin123");
        var admin = new User
        {
            Username = "admin",
            PasswordHash = hash,
            PasswordSalt = salt,
            RoleId = supervisorRole.Id
        };

        await _userRepository.CreateAsync(admin, ct);
    }
}