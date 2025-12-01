using Incident.Infrastructure;
using Incident.Models;
using Incident.Repositories;

namespace Incident.Services;

public class DbSeederService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public DbSeederService(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task SeedAsync()
    {
        // Seed Admin/Supervisor user
        await SeedUserAsync("admin", "supervisor");
        
        // Seed Secretary user
        await SeedUserAsync("secretary", "secretary");
        
        // Seed Officer user
        await SeedUserAsync("officer", "officer");
        
        // Seed Supervisor user (separate from admin)
        await SeedUserAsync("supervisor", "supervisor");
    }

    private async Task SeedUserAsync(string username, string roleName)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(username);
        if (existingUser != null)
        {
            return; // User already exists
        }

        var role = await _roleRepository.GetByNameAsync(roleName);
        if (role == null)
        {
            Console.WriteLine($"Warning: Role '{roleName}' not found. Skipping user '{username}' creation.");
            return;
        }

        var (hash, salt) = PasswordHasher.HashPassword("admin123");
        var user = new User
        {
            Username = username,
            PasswordHash = hash,
            PasswordSalt = salt,
            RoleId = role.Id
        };

        await _userRepository.CreateAsync(user);
        Console.WriteLine($"User '{username}' created with role '{roleName}'.");
    }
}