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

        var status = await _statusRepository.GetByIdAsync(incident.StatusId);
        if (status == null)
        {
            return new IncidentActionFlags();
        }

        var currentUser = await _userRepository.GetByIdAsync(userId);
        if (currentUser == null)
        {
            return new IncidentActionFlags();
        }

        bool isCreator = incident.CreatedByUserId == userId;
        bool isAssignedOfficer = incident.SentToUserId == userId;
        bool isOfficer = currentUser.Role?.Code == "officer";

        var nextActions = new List<string>();
        bool canSendToReview = false;
        bool canAccept = false;
        bool canReject = false;
        bool canEdit = false;

        // Creator actions
        if (isCreator)
        {
            canEdit = status.Code == "draft" || status.Code == "rejected";

            if (status.Code == "draft" || status.Code == "rejected")
            {
                nextActions.Add("send_to_review");
                canSendToReview = true;
            }
        }

        // Officer actions
        if (isOfficer && isAssignedOfficer)
        {
            if (status.Code == "in_review")
            {
                nextActions.Add("accept");
                nextActions.Add("reject");
                canAccept = true;
                canReject = true;
            }
        }

        return new IncidentActionFlags
        {
            NextActions = nextActions,
            CanSendToReview = canSendToReview,
            CanAccept = canAccept,
            CanReject = canReject,
            CanEdit = canEdit
        };
    }

    public async Task<StatusUpdateResult> UpdateStatusAsync(Guid incidentId, Guid userId, IncidentStatusUpdateRequestDto request)
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

        var currentUser = await _userRepository.GetByIdAsync(userId);
        if (currentUser == null)
        {
            return new StatusUpdateResult
            {
                Success = false,
                ErrorMessage = "User not found"
            };
        }

        // Validate the action
        var allowedTransitions = await _statusRepository.GetAllowedTransitionsAsync(incident.StatusId, request.Action, userId, incident.CreatedByUserId, incident.SentToUserId);
        if (!allowedTransitions.Any())
        {
            return new StatusUpdateResult
            {
                Success = false,
                ErrorMessage = $"Action '{request.Action}' is not allowed from current status"
            };
        }

        var transition = allowedTransitions.First();
        var targetStatus = await _statusRepository.GetByIdAsync(transition.ToStatusId);
        if (targetStatus == null)
        {
            return new StatusUpdateResult
            {
                Success = false,
                ErrorMessage = "Target status not found"
            };
        }

        // Update incident status
        Guid? newSentToUserId = incident.SentToUserId;
        if (request.Action == "send_to_review" && request.NewSentToUserId.HasValue)
        {
            // Verify the target user is an officer
            var targetUser = await _userRepository.GetByIdAsync(request.NewSentToUserId.Value);
            if (targetUser?.Role?.Code != "officer")
            {
                return new StatusUpdateResult
                {
                    Success = false,
                    ErrorMessage = "Target user must be an officer"
                };
            }
            newSentToUserId = request.NewSentToUserId.Value;
        }

        var success = await _incidentRepository.UpdateStatusAsync(incidentId, targetStatus.Id, newSentToUserId);
        if (!success)
        {
            return new StatusUpdateResult
            {
                Success = false,
                ErrorMessage = "Failed to update incident status"
            };
        }

        // Update the incident object with new status
        incident.StatusId = targetStatus.Id;
        incident.SentToUserId = newSentToUserId;
        incident.UpdatedAt = DateTime.UtcNow;

        // Get updated action flags
        var actionFlags = await GetActionFlagsAsync(incidentId, userId);

        return new StatusUpdateResult
        {
            Success = true,
            Incident = incident,
            ActionFlags = actionFlags
        };
    }
}