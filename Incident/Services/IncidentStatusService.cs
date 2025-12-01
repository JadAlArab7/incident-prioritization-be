using Incident.DTOs;
using Incident.Models;
using Incident.Repositories;

namespace Incident.Services;

public class IncidentStatusService : IIncidentStatusService
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IIncidentStatusRepository _statusRepository;
    private readonly IUserRepository _userRepository;

    public IncidentStatusService(
        IIncidentRepository incidentRepository,
        IIncidentStatusRepository statusRepository,
        IUserRepository userRepository)
    {
        _incidentRepository = incidentRepository;
        _statusRepository = statusRepository;
        _userRepository = userRepository;
    }

    public async Task<IncidentActionFlags> GetActionFlagsAsync(Guid incidentId, Guid userId)
    {
        var incident = await _incidentRepository.GetByIdAsync(incidentId);
        if (incident == null)
        {
            return new IncidentActionFlags();
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return new IncidentActionFlags();
        }

        var currentStatus = await _statusRepository.GetByIdAsync(incident.StatusId);
        if (currentStatus == null)
        {
            return new IncidentActionFlags();
        }

        var transitions = await _statusRepository.GetTransitionsFromStatusAsync(incident.StatusId);
        var nextActions = new List<string>();

        bool isCreator = incident.CreatedByUserId == userId;
        bool isAssignedOfficer = incident.SentToUserId == userId;
        bool isOfficer = user.Role?.Code == "officer";

        foreach (var transition in transitions)
        {
            if (transition.Initiator == "creator" && isCreator)
            {
                nextActions.Add(transition.ActionCode);
            }
            else if (transition.Initiator == "officer" && isAssignedOfficer && isOfficer)
            {
                nextActions.Add(transition.ActionCode);
            }
        }

        bool canEdit = isCreator && (currentStatus.Code == "draft" || currentStatus.Code == "rejected");

        return new IncidentActionFlags
        {
            NextActions = nextActions,
            CanSendToReview = nextActions.Contains("send_to_review"),
            CanAccept = nextActions.Contains("accept"),
            CanReject = nextActions.Contains("reject"),
            CanEdit = canEdit
        };
    }

    public async Task<StatusUpdateResult> UpdateStatusAsync(
        Guid incidentId,
        Guid userId,
        IncidentStatusUpdateRequestDto request)
    {
        var incident = await _incidentRepository.GetByIdAsync(incidentId);
        if (incident == null)
        {
            return new StatusUpdateResult
            {
                Success = false,
                ErrorMessage = "Incident not found"
            };
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return new StatusUpdateResult
            {
                Success = false,
                ErrorMessage = "User not found"
            };
        }

        var currentStatus = await _statusRepository.GetByIdAsync(incident.StatusId);
        if (currentStatus == null)
        {
            return new StatusUpdateResult
            {
                Success = false,
                ErrorMessage = "Current status not found"
            };
        }

        // Find the valid transition
        var transition = await _statusRepository.GetTransitionAsync(
            incident.StatusId,
            request.Action);

        if (transition == null)
        {
            return new StatusUpdateResult
            {
                Success = false,
                ErrorMessage = $"Action '{request.Action}' is not allowed from status '{currentStatus.Code}'"
            };
        }

        // Check authorization
        bool isCreator = incident.CreatedByUserId == userId;
        bool isAssignedOfficer = incident.SentToUserId == userId;
        bool isOfficer = user.Role?.Code == "officer";

        if (transition.Initiator == "creator" && !isCreator)
        {
            return new StatusUpdateResult
            {
                Success = false,
                ErrorMessage = "Only the incident creator can perform this action"
            };
        }

        if (transition.Initiator == "officer" && (!isAssignedOfficer || !isOfficer))
        {
            return new StatusUpdateResult
            {
                Success = false,
                ErrorMessage = "Only the assigned officer can perform this action"
            };
        }

        // Handle send_to_review action - requires newSentToUserId
        Guid? newSentToUserId = incident.SentToUserId;
        if (request.Action == "send_to_review")
        {
            if (request.NewSentToUserId == null && incident.SentToUserId == null)
            {
                return new StatusUpdateResult
                {
                    Success = false,
                    ErrorMessage = "newSentToUserId is required when sending to review"
                };
            }

            if (request.NewSentToUserId != null)
            {
                var targetUser = await _userRepository.GetByIdAsync(request.NewSentToUserId.Value);
                if (targetUser == null)
                {
                    return new StatusUpdateResult
                    {
                        Success = false,
                        ErrorMessage = "Target user not found"
                    };
                }

                if (targetUser.Role?.Code != "officer")
                {
                    return new StatusUpdateResult
                    {
                        Success = false,
                        ErrorMessage = "Target user must have the officer role"
                    };
                }

                newSentToUserId = request.NewSentToUserId;
            }
        }

        // Update the incident status
        var success = await _incidentRepository.UpdateStatusAsync(
            incidentId,
            transition.ToStatusId,
            newSentToUserId);

        if (!success)
        {
            return new StatusUpdateResult
            {
                Success = false,
                ErrorMessage = "Failed to update incident status. It may have been modified by another user."
            };
        }

        // Record the status change in history
        await _statusRepository.AddStatusHistoryAsync(new IncidentStatusHistory
        {
            Id = Guid.NewGuid(),
            IncidentId = incidentId,
            FromStatusId = incident.StatusId,
            ToStatusId = transition.ToStatusId,
            ChangedByUserId = userId,
            Comment = request.Comment,
            ChangedAt = DateTime.UtcNow
        });

        // Get updated incident
        var updatedIncident = await _incidentRepository.GetByIdAsync(incidentId);
        var actionFlags = await GetActionFlagsAsync(incidentId, userId);

        return new StatusUpdateResult
        {
            Success = true,
            Incident = updatedIncident,
            ActionFlags = actionFlags
        };
    }

    public async Task<IncidentDetailResponseDto?> GetIncidentWithActionsAsync(Guid incidentId, Guid userId)
    {
        var incident = await _incidentRepository.GetByIdAsync(incidentId);
        if (incident == null)
        {
            return null;
        }

        var actionFlags = await GetActionFlagsAsync(incidentId, userId);
        var status = await _statusRepository.GetByIdAsync(incident.StatusId);

        return new IncidentDetailResponseDto
        {
            Id = incident.Id,
            Title = incident.Title,
            Description = incident.Description,
            IncidentTypeId = incident.IncidentTypeId,
            IncidentTypeName = incident.IncidentTypeName,
            StatusId = incident.StatusId,
            StatusCode = status?.Code ?? "",
            StatusName = status?.Name ?? "",
            LocationId = incident.LocationId,
            Location = incident.Location,
            CreatedByUserId = incident.CreatedByUserId,
            CreatedByUserName = incident.CreatedByUserName,
            SentToUserId = incident.SentToUserId,
            SentToUserName = incident.SentToUserName,
            CreatedAt = incident.CreatedAt,
            UpdatedAt = incident.UpdatedAt,
            NextActions = actionFlags.NextActions,
            CanSendToReview = actionFlags.CanSendToReview,
            CanAccept = actionFlags.CanAccept,
            CanReject = actionFlags.CanReject,
            CanEdit = actionFlags.CanEdit
        };
    }
}