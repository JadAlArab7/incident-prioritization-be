using Incident.Models;

namespace Incident.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string name);
    Task<Role?> GetByIdAsync(Guid id);
    Task<List<Role>> GetAllAsync();
}