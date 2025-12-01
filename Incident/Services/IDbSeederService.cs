namespace Incident.Services;

public interface IDbSeederService
{
    Task SeedAsync(CancellationToken ct = default);
    Task SeedRolesAsync(CancellationToken ct = default);
    Task SeedUsersAsync(CancellationToken ct = default);
    Task SeedIncidentStatusesAsync(CancellationToken ct = default);
    Task SeedIncidentTypesAsync(CancellationToken ct = default);
    Task SeedStatusTransitionsAsync(CancellationToken ct = default);
}