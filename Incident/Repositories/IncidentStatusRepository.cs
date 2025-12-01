using Incident.Infrastructure;
using Incident.Models;

namespace Incident.Repositories;

public class IncidentStatusRepository : IIncidentStatusRepository
{
    private readonly IDbHelper _dbHelper;

    public IncidentStatusRepository(IDbHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public async Task<IncidentStatus?> GetByIdAsync(Guid id)
    {
        const string sql = @"
            SELECT id, code, name, description, is_terminal as IsTerminal
            FROM incident_statuses
            WHERE id = @Id";

        return await _dbHelper.QuerySingleOrDefaultAsync<IncidentStatus>(sql, new { Id = id });
    }

    public async Task<IncidentStatus?> GetByCodeAsync(string code)
    {
        const string sql = @"
            SELECT id, code, name, description, is_terminal as IsTerminal
            FROM incident_statuses
            WHERE code = @Code";

        return await _dbHelper.QuerySingleOrDefaultAsync<IncidentStatus>(sql, new { Code = code });
    }

    public async Task<IEnumerable<IncidentStatusTransition>> GetAllowedTransitionsAsync(
        Guid fromStatusId, 
        string action, 
        Guid userId, 
        Guid incidentCreatorId, 
        Guid? incidentAssignedOfficerId)
    {
        const string sql = @"
            SELECT ist.id, ist.from_status_id as FromStatusId, ist.to_status_id as ToStatusId, 
                   ist.action_code as ActionCode, ist.initiator, ist.is_active as IsActive
            FROM incident_status_transitions ist
            WHERE ist.from_status_id = @FromStatusId 
              AND ist.action_code = @ActionCode
              AND ist.is_active = true
              AND (
                (ist.initiator = 'creator' AND @UserId = @IncidentCreatorId) OR
                (ist.initiator = 'officer' AND @IncidentAssignedOfficerId = @UserId)
              )";

        return await _dbHelper.QueryAsync<IncidentStatusTransition>(sql, new
        {
            FromStatusId = fromStatusId,
            ActionCode = action,
            UserId = userId,
            IncidentCreatorId = incidentCreatorId,
            IncidentAssignedOfficerId = incidentAssignedOfficerId
        });
    }
}