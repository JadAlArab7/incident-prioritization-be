namespace Incident.DTOs;

public class IncidentResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public UserSummaryDto? SentToUser { get; set; }
    public UserSummaryDto CreatedByUser { get; set; } = null!;
    public LocationResponseDto? Location { get; set; }
    public string? Priority { get; set; }
    public string? SuggestedActionsTaken { get; set; }
    public IncidentStatusDto Status { get; set; } = null!;
    public List<IncidentTypeDto> Types { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public IncidentWorkflowActionsDto AvailableActions { get; set; } = null!;
}

public class UserSummaryDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? RoleName { get; set; }
}

public class LocationResponseDto
{
    public Guid Id { get; set; }
    public double Lat { get; set; }
    public double Lng { get; set; }
    public GeoLookupDto? Governorate { get; set; }
    public GeoLookupDto? District { get; set; }
    public GeoLookupDto? Town { get; set; }
    public string? AddressText { get; set; }
}

public class GeoLookupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
}

public class IncidentStatusDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
}

public class IncidentTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
}

public class IncidentWorkflowActionsDto
{
    public bool CanSendToReview { get; set; }
    public bool CanSendToAccept { get; set; }
    public bool CanSendToReject { get; set; }
}
