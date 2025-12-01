namespace Incident.DTOs;

public class StatusUpdateResult
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string? ErrorMessage { get; set; }
    public IncidentStatusUpdateResponseDto? Data { get; set; }

    public static StatusUpdateResult Ok(IncidentStatusUpdateResponseDto data)
    {
        return new StatusUpdateResult
        {
            Success = true,
            StatusCode = 200,
            Data = data
        };
    }

    public static StatusUpdateResult NotFound(string message)
    {
        return new StatusUpdateResult
        {
            Success = false,
            StatusCode = 404,
            ErrorMessage = message
        };
    }

    public static StatusUpdateResult BadRequest(string message)
    {
        return new StatusUpdateResult
        {
            Success = false,
            StatusCode = 400,
            ErrorMessage = message
        };
    }

    public static StatusUpdateResult Forbidden(string message)
    {
        return new StatusUpdateResult
        {
            Success = false,
            StatusCode = 403,
            ErrorMessage = message
        };
    }

    public static StatusUpdateResult Conflict(string message)
    {
        return new StatusUpdateResult
        {
            Success = false,
            StatusCode = 409,
            ErrorMessage = message
        };
    }
}