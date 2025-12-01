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

    public async Task<IEnumerable<IncidentStatus>> GetAllAsync()
    {
        const string sql = @"
            SELECT id, code, name, description, is_terminal as IsTerminal
            FROM incident_statuses
            ORDER BY name";

        return await _dbHelper.QueryAsync<IncidentStatus>(sql);
    }

    public async Task<IEnumerable<IncidentStatusTransition>> GetTransitionsFromStatusAsync(Guid fromStatusId)
    {
        const string sql = @"
            SELECT id, from_status_id as FromStatusId, to_status_id as ToStatusId, 
                   action_code as ActionCode, initiator, is_active as IsActive
            FROM incident_status_transitions
            WHERE from_status_id = @FromStatusId AND is_active = true";

        return await _dbHelper.QueryAsync<IncidentStatusTransition>(sql, new { FromStatusId = fromStatusId });
    }

    public async Task<IncidentStatusTransition?> GetTransitionAsync(Guid fromStatusId, string actionCode)
    {
        const string sql = @"
            SELECT id, from_status_id as FromStatusId, to_status_id as ToStatusId, 
                   action_code as ActionCode, initiator, is_active as IsActive
            FROM incident_status_transitions
            WHERE from_status_id = @FromStatusId AND action_code = @ActionCode AND is_active = true";

        return await _dbHelper.QuerySingleOrDefaultAsync<IncidentStatusTransition>(
            sql, 
            new { FromStatusId = fromStatusId, ActionCode = actionCode });
    }

    public async Task AddStatusHistoryAsync(IncidentStatusHistory history)
    {
        const string sql = @"
            INSERT INTO incident_status_history 
                (id, incident_id, from_status_id, to_status_id, changed_by_user_id, comment, changed_at)
            VALUES 
                (@Id, @IncidentId, @FromStatusId, @ToStatusId, @ChangedByUserId, @Comment, @ChangedAt)";

        await _dbHelper.ExecuteAsync(sql, new
        {
            history.Id,
            history.IncidentId,
            history.FromStatusId,
            history.ToStatusId,
            history.ChangedByUserId,
            history.Comment,
            history.ChangedAt
        });
    }

    public async Task<IEnumerable<IncidentStatusHistory>> GetStatusHistoryAsync(Guid incidentId)
    {
        const string sql = @"
            SELECT h.id, h.incident_id as IncidentId, h.from_status_id as FromStatusId, 
                   h.to_status_id as ToStatusId, h.changed_by_user_id as ChangedByUserId, 
                   h.comment, h.changed_at as ChangedAt,
                   u.full_name as ChangedByUserName,
                   fs.name as FromStatusName,
                   ts.name as ToStatusName
            FROM incident_status_history h
            LEFT JOIN users u ON h.changed_by_user_id = u.id
            LEFT JOIN incident_statuses fs ON h.from_status_id = fs.id
            LEFT JOIN incident_statuses ts ON h.to_status_id = ts.id
            WHERE h.incident_id = @IncidentId
            ORDER BY h.changed_at DESC";

        return await _dbHelper.QueryAsync<IncidentStatusHistory>(sql, new { IncidentId = incidentId });
    }
}