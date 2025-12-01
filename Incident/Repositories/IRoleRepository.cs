using Incident.Models;

namespace Incident.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id);
    Task<Role?> GetByCodeAsync(string code);
    Task<IEnumerable<Role>> GetAllAsync();
    Task<Role> CreateAsync(Role role);
    Task<Role?> UpdateAsync(Role role);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> CodeExistsAsync(string code);
}