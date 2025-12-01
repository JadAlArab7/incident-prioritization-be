namespace Incident.DTOs;

public class PagedRequestDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    
    public int Offset => (Page - 1) * PageSize;
}