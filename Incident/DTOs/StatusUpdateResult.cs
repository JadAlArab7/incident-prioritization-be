namespace Incident.DTOs;

public sealed class StatusUpdateResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int? ErrorCode { get; set; } // 400, 403, 404, 409
    public IncidentStatusUpdateResponseDto? Response { get; set; }

    public static StatusUpdateResult Ok(IncidentStatusUpdateResponseDto response) => new()
    {
        Success = true,
        Response = response
    };

    public static StatusUpdateResult BadRequest(string message) => new()
    {
        Success = false,
        ErrorMessage = message,
        ErrorCode = 400
    };

    public static StatusUpdateResult Forbidden(string message) => new()
    {
        Success = false,
        ErrorMessage = message,
        ErrorCode = 403
    };

    public static StatusUpdateResult NotFound(string message) => new()
    {
        Success = false,
        ErrorMessage = message,
        ErrorCode = 404
    };

    public static StatusUpdateResult Conflict(string message) => new()
    {
        Success = false,
        ErrorMessage = message,
        ErrorCode = 409
    };
}