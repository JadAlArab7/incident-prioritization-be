using Incident.DTOs;
using Incident.Models;
using Incident.Repositories;
using Incident.Infrastructure;
using Npgsql;

namespace Incident.Services;

public class IncidentStatusService : IIncidentStatusService
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IIncidentStatusRepository _statusRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDbHelper _dbHelper;
    private readonly ILogger<IncidentStatusService> _logger;

    private const string ROLE_OFFICER = "officer";
    private const string INITIATOR_CREATOR = "creator";
    private const string INITIATOR_OFFICER = "officer";

    public IncidentStatusService(
        IIncidentRepository incidentRepository,
        IIncidentStatusRepository statusRepository,
        IUserRepository userRepository,
        IDbHelper dbHelper,
        ILogger<IncidentStatusService> logger)
    {
        _incidentRepository = incidentRepository;
        _statusRepository = statusRepository;
        _userRepository = userRepository;
        _dbHelper = dbHelper;
        _logger = logger;
    }

    public async Task<StatusUpdateResult> UpdateStatusAsync(
        Guid incidentId,
        Guid currentUserId,
        string actionCode,
        string? comment,
        Guid? newSentToUserId,
        CancellationToken ct = default)
    {
        // 1. Load incident
        var incident = await _incidentRepository.GetByIdAsync(incidentId, ct);
        if (incident == null)
        {
            return StatusUpdateResult.NotFound($"Incident with ID {incidentId} not found");
        }

        // 2. Load current user with role
        var currentUser = await _userRepository.GetByIdAsync(currentUserId, ct);
        if (currentUser == null)
        {
            return StatusUpdateResult.Forbidden("Current user not found");
        }

        // 3. Lookup transition
        var transition = await _statusRepository.GetTransitionAsync(incident.StatusId, actionCode, ct);
        if (transition == null)
        {
            var currentStatusCode = await _statusRepository.GetStatusCodeByIdAsync(incident.StatusId, ct);
            return StatusUpdateResult.BadRequest($"Action '{actionCode}' is not allowed from status '{currentStatusCode}'");
        }

        // 4. Validate initiator
        var validationResult = await ValidateInitiatorAsync(transition, incident, currentUserId, currentUser, ct);
        if (!validationResult.Success)
        {
            return validationResult;
        }

        // 5. Handle send_to_review specific logic
        Guid? finalSentToUserId = incident.SentToUserId;
        if (actionCode == "send_to_review")
        {
            var sendToReviewResult = await ValidateSendToReviewAsync(incident, newSentToUserId, ct);
            if (!sendToReviewResult.Success)
            {
                return sendToReviewResult;
            }
            finalSentToUserId = newSentToUserId ?? incident.SentToUserId;
        }

        // 6. Execute transaction
        try
        {
            await ExecuteStatusTransitionAsync(
                incident,
                transition.ToStatusId,
                currentUserId,
                comment,
                finalSentToUserId,
                ct);
        }
        catch (NpgsqlException ex) when (ex.Message.Contains("sent_to_user_id is required"))
        {
            return StatusUpdateResult.BadRequest("sent_to_user_id is required when sending to review");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update incident status");
            throw;
        }

        // 7. Load updated incident and compute flags
        var updatedIncident = await _incidentRepository.GetByIdAsync(incidentId, ct);
        var incidentResponse = MapToResponseDto(updatedIncident!);
        var flags = await ComputeActionFlagsAsync(incidentId, currentUserId, ct);

        return StatusUpdateResult.Ok(new IncidentStatusUpdateResponseDto
        {
            Incident = incidentResponse,
            NextActions = flags.NextActions,
            CanSendToReview = flags.CanSendToReview,
            CanAccept = flags.CanAccept,
            CanReject = flags.CanReject,
            CanEdit = flags.CanEdit
        });
    }

    public async Task<IncidentActionFlags> ComputeActionFlagsAsync(
        Guid incidentId,
        Guid currentUserId,
        CancellationToken ct = default)
    {
        var incident = await _incidentRepository.GetByIdAsync(incidentId, ct);
        if (incident == null)
        {
            return new IncidentActionFlags();
        }

        var currentUser = await _userRepository.GetByIdAsync(currentUserId, ct);
        if (currentUser == null)
        {
            return new IncidentActionFlags();
        }

        var statusCode = await _statusRepository.GetStatusCodeByIdAsync(incident.StatusId, ct);
        var transitions = await _statusRepository.GetTransitionsFromStatusAsync(incident.StatusId, ct);

        var allowedActions = new List<string>();

        foreach (var transition in transitions)
        {
            bool isAllowed = false;

            if (transition.Initiator == INITIATOR_CREATOR)
            {
                isAllowed = currentUserId == incident.CreatedByUserId;
            }
            else if (transition.Initiator == INITIATOR_OFFICER)
            {
                isAllowed = currentUserId == incident.SentToUserId && 
                           currentUser.RoleCode?.ToLower() == ROLE_OFFICER;
            }

            if (isAllowed)
            {
                allowedActions.Add(transition.ActionCode);
            }
        }

        // Compute canEdit: creator can edit while draft or rejected
        bool canEdit = currentUserId == incident.CreatedByUserId &&
                      (statusCode == "draft" || statusCode == "rejected");

        return new IncidentActionFlags
        {
            NextActions = allowedActions,
            CanSendToReview = allowedActions.Contains("send_to_review"),
            CanAccept = allowedActions.Contains("accept"),
            CanReject = allowedActions.Contains("reject"),
            CanEdit = canEdit
        };
    }

    public async Task<IncidentDetailResponseDto?> GetIncidentWithFlagsAsync(
        Guid incidentId,
        Guid currentUserId,
        CancellationToken ct = default)
    {
        var incident = await _incidentRepository.GetByIdAsync(incidentId, ct);
        if (incident == null)
        {
            return null;
        }

        var flags = await ComputeActionFlagsAsync(incidentId, currentUserId, ct);
        var statusCode = await _statusRepository.GetStatusCodeByIdAsync(incident.StatusId, ct);

        return new IncidentDetailResponseDto
        {
            Id = incident.Id,
            Title = incident.Title,
            Description = incident.Description,
            IncidentTypeId = incident.IncidentTypeId,
            IncidentTypeName = incident.IncidentTypeName,
            StatusId = incident.StatusId,
            StatusCode = statusCode,
            StatusName = incident.StatusName,
            LocationId = incident.LocationId,
            Location = incident.Location != null ? new LocationDto
            {
                Id = incident.Location.Id,
                GovernorateId = incident.Location.GovernorateId,
                GovernorateName = incident.Location.GovernorateName,
                DistrictId = incident.Location.DistrictId,
                DistrictName = incident.Location.DistrictName,
                TownId = incident.Location.TownId,
                TownName = incident.Location.TownName
            } : null,
            CreatedByUserId = incident.CreatedByUserId,
            CreatedByUserName = incident.CreatedByUserName,
            SentToUserId = incident.SentToUserId,
            SentToUserName = incident.SentToUserName,
            CreatedAt = incident.CreatedAt,
            UpdatedAt = incident.UpdatedAt,
            NextActions = flags.NextActions,
            CanSendToReview = flags.CanSendToReview,
            CanAccept = flags.CanAccept,
            CanReject = flags.CanReject,
            CanEdit = flags.CanEdit
        };
    }

    private async Task<StatusUpdateResult> ValidateInitiatorAsync(
        IncidentStatusTransition transition,
        IncidentRecord incident,
        Guid currentUserId,
        User currentUser,
        CancellationToken ct)
    {
        if (transition.Initiator == INITIATOR_CREATOR)
        {
            if (currentUserId != incident.CreatedByUserId)
            {
                return StatusUpdateResult.Forbidden("Only the incident creator can perform this action");
            }
        }
        else if (transition.Initiator == INITIATOR_OFFICER)
        {
            if (currentUserId != incident.SentToUserId)
            {
                return StatusUpdateResult.Forbidden("Only the assigned officer can perform this action");
            }

            if (currentUser.RoleCode?.ToLower() != ROLE_OFFICER)
            {
                return StatusUpdateResult.Forbidden("User must have officer role to perform this action");
            }
        }

        return StatusUpdateResult.Ok(null!);
    }

    private async Task<StatusUpdateResult> ValidateSendToReviewAsync(
        IncidentRecord incident,
        Guid? newSentToUserId,
        CancellationToken ct)
    {
        var targetUserId = newSentToUserId ?? incident.SentToUserId;

        if (targetUserId == null)
        {
            return StatusUpdateResult.BadRequest("newSentToUserId is required when sending to review");
        }

        // Validate that the target user exists and has officer role
        var targetUser = await _userRepository.GetByIdAsync(targetUserId.Value, ct);
        if (targetUser == null)
        {
            return StatusUpdateResult.BadRequest($"User with ID {targetUserId} not found");
        }

        if (targetUser.RoleCode?.ToLower() != ROLE_OFFICER)
        {
            return StatusUpdateResult.BadRequest($"User {targetUserId} must have officer role to be assigned");
        }

        return StatusUpdateResult.Ok(null!);
    }

    private async Task ExecuteStatusTransitionAsync(
        IncidentRecord incident,
        Guid toStatusId,
        Guid changedByUserId,
        string? comment,
        Guid? sentToUserId,
        CancellationToken ct)
    {
        var connectionString = _dbHelper.GetConnectionString();
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(ct);
        await using var transaction = await connection.BeginTransactionAsync(ct);

        try
        {
            // Lock and update incident
            const string updateSql = @"
                UPDATE incidents 
                SET status_id = @statusId, 
                    sent_to_user_id = @sentToUserId,
                    updated_at = NOW()
                WHERE id = @id";

            await using var updateCmd = new NpgsqlCommand(updateSql, connection, transaction);
            updateCmd.Parameters.AddWithValue("@statusId", toStatusId);
            updateCmd.Parameters.AddWithValue("@sentToUserId", (object?)sentToUserId ?? DBNull.Value);
            updateCmd.Parameters.AddWithValue("@id", incident.Id);
            await updateCmd.ExecuteNonQueryAsync(ct);

            // Insert history record
            const string historySql = @"
                INSERT INTO incident_status_history (id, incident_id, from_status_id, to_status_id, changed_by_user_id, comment, changed_at)
                VALUES (@id, @incidentId, @fromStatusId, @toStatusId, @changedByUserId, @comment, NOW())";

            await using var historyCmd = new NpgsqlCommand(historySql, connection, transaction);
            historyCmd.Parameters.AddWithValue("@id", Guid.NewGuid());
            historyCmd.Parameters.AddWithValue("@incidentId", incident.Id);
            historyCmd.Parameters.AddWithValue("@fromStatusId", incident.StatusId);
            historyCmd.Parameters.AddWithValue("@toStatusId", toStatusId);
            historyCmd.Parameters.AddWithValue("@changedByUserId", changedByUserId);
            historyCmd.Parameters.AddWithValue("@comment", (object?)comment ?? DBNull.Value);
            await historyCmd.ExecuteNonQueryAsync(ct);

            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    private static IncidentResponseDto MapToResponseDto(IncidentRecord incident)
    {
        return new IncidentResponseDto
        {
            Id = incident.Id,
            Title = incident.Title,
            Description = incident.Description,
            IncidentTypeId = incident.IncidentTypeId,
            IncidentTypeName = incident.IncidentTypeName,
            StatusId = incident.StatusId,
            StatusName = incident.StatusName,
            LocationId = incident.LocationId,
            Location = incident.Location != null ? new LocationDto
            {
                Id = incident.Location.Id,
                GovernorateId = incident.Location.GovernorateId,
                GovernorateName = incident.Location.GovernorateName,
                DistrictId = incident.Location.DistrictId,
                DistrictName = incident.Location.DistrictName,
                TownId = incident.Location.TownId,
                TownName = incident.Location.TownName
            } : null,
            CreatedByUserId = incident.CreatedByUserId,
            CreatedByUserName = incident.CreatedByUserName,
            SentToUserId = incident.SentToUserId,
            SentToUserName = incident.SentToUserName,
            CreatedAt = incident.CreatedAt,
            UpdatedAt = incident.UpdatedAt
        };
    }
}