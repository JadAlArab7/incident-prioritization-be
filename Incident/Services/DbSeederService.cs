using Incident.DTOs;

namespace Incident.Services;

public interface IDbSeederService
{
    Task SeedAdminUserAsync();
}

public class DbSeederService : IDbSeederService
{
    private readonly IUserService _userService;
    private readonly ILogger<DbSeederService> _logger;

    public DbSeederService(IUserService userService, ILogger<DbSeederService> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task SeedAdminUserAsync()
    {
        const string adminUsername = "admin";
        const string adminPassword = "Admin@123";
        const string adminRole = "supervisor";

        if (await _userService.UserExistsAsync(adminUsername))
        {
            _logger.LogInformation("Admin user already exists. Skipping seed.");
            return;
        }

        var createUserDto = new CreateUserDto
        {
            Username = adminUsername,
            Password = adminPassword,
            RoleName = adminRole
        };

        var userId = await _userService.CreateUserAsync(createUserDto);
        _logger.LogInformation("Admin user created with ID: {UserId}", userId);
    }
}