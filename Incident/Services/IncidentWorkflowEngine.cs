using Incident.Models;

namespace Incident.Services;

public static class IncidentWorkflowEngine
{
    private static readonly Dictionary<string, IncidentWorkflowActions> WorkflowMap = new()
    {
        ["draft"] = new IncidentWorkflowActions
        {
            CanSendToReview = true,
            CanSendToAccept = false,
            CanSendToReject = false
        },
        ["in_review"] = new IncidentWorkflowActions
        {
            CanSendToReview = false,
            CanSendToAccept = true,
            CanSendToReject = true
        },
        ["accepted"] = new IncidentWorkflowActions
        {
            CanSendToReview = false,
            CanSendToAccept = false,
            CanSendToReject = false
        },
        ["rejected"] = new IncidentWorkflowActions
        {
            CanSendToReview = true,
            CanSendToAccept = false,
            CanSendToReject = false
        }
    };

    public static IncidentWorkflowActions GetAvailableActions(string statusCode)
    {
        return WorkflowMap.TryGetValue(statusCode, out var actions)
            ? actions
            : new IncidentWorkflowActions(); // All false by default
    }
}

public class IncidentWorkflowActions
{
    public bool CanSendToReview { get; set; }
    public bool CanSendToAccept { get; set; }
    public bool CanSendToReject { get; set; }
}
