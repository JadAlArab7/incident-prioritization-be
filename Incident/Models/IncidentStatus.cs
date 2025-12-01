namespace Incident.Models;

public class IncidentStatus
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    
    // Well-known status codes
    public static readonly string Draft = "draft";
    public static readonly string InReview = "in_review";
    public static readonly string Accepted = "accepted";
    public static readonly string Rejected = "rejected";
    
    // Well-known status IDs
    public static readonly Guid DraftId = Guid.Parse("00000000-0000-0000-0000-00000000d001");
    public static readonly Guid InReviewId = Guid.Parse("00000000-0000-0000-0000-00000000d002");
    public static readonly Guid AcceptedId = Guid.Parse("00000000-0000-0000-0000-00000000d003");
    public static readonly Guid RejectedId = Guid.Parse("00000000-0000-0000-0000-00000000d004");
}