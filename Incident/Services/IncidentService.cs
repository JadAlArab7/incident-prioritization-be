using Incident.DTOs;
using Incident.Models;
using Incident.Repositories;

namespace Incident.Services;

public class IncidentService : IIncidentService
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly ILookupRepository _lookupRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILlmService _llmService;

    public IncidentService(
        IIncidentRepository incidentRepository,
        ILookupRepository lookupRepository,
        IUserRepository userRepository,
        ILlmService llmService)
    {
        _incidentRepository = incidentRepository;
        _lookupRepository = lookupRepository;
        _userRepository = userRepository;
        _llmService = llmService;
    }

    public async Task<IncidentResponseDto> CreateAsync(IncidentCreateRequestDto request, Guid currentUserId, CancellationToken ct = default)
    {
        // Validate sent_to_user exists if provided
        if (request.SentToUserId.HasValue)
        {
            var sentToUser = await _userRepository.GetByIdAsync(request.SentToUserId.Value, ct);
            if (sentToUser == null)
                throw new ArgumentException("Sent to user not found");
        }

        // Create location if provided
        Guid? locationId = null;
        if (request.Location != null)
        {
            var location = new Location
            {
                Lat = request.Location.Lat,
                Lng = request.Location.Lng,
                GovernorateId = request.Location.GovernorateId,
                DistrictId = request.Location.DistrictId,
                TownId = request.Location.TownId,
                AddressText = request.Location.AddressText
            };
            locationId = await _incidentRepository.CreateLocationAsync(location, ct);
        }

        // Default status is 'draft'
        var incident = new IncidentRecord
        {
            Title = request.Title,
            Description = request.Description,
            SentToUserId = request.SentToUserId,
            CreatedByUserId = currentUserId,
            LocationId = locationId,
            Priority = request.Priority,
            SuggestedActionsTaken = request.SuggestedActionsTaken,
            StatusId = IncidentStatus.DraftId
        };

        var id = await _incidentRepository.CreateAsync(incident, request.TypeIds, ct);
        
        return (await GetByIdAsync(id, ct))!;
    }

    public async Task<IncidentResponseDto> UpdateAsync(Guid id, IncidentUpdateRequestDto request, Guid currentUserId, CancellationToken ct = default)
    {
        var existing = await _incidentRepository.GetByIdAsync(id, ct);
        if (existing == null)
            throw new KeyNotFoundException("Incident not found");

        // Validate sent_to_user exists if provided
        if (request.SentToUserId.HasValue)
        {
            var sentToUser = await _userRepository.GetByIdAsync(request.SentToUserId.Value, ct);
            if (sentToUser == null)
                throw new ArgumentException("Sent to user not found");
        }

        // Handle location update
        Guid? locationId = existing.LocationId;
        if (request.Location != null)
        {
            if (existing.LocationId.HasValue)
            {
                // Update existing location
                var location = new Location
                {
                    Id = existing.LocationId.Value,
                    Lat = request.Location.Lat,
                    Lng = request.Location.Lng,
                    GovernorateId = request.Location.GovernorateId,
                    DistrictId = request.Location.DistrictId,
                    TownId = request.Location.TownId,
                    AddressText = request.Location.AddressText
                };
                await _incidentRepository.UpdateLocationAsync(location, ct);
            }
            else
            {
                // Create new location
                var location = new Location
                {
                    Lat = request.Location.Lat,
                    Lng = request.Location.Lng,
                    GovernorateId = request.Location.GovernorateId,
                    DistrictId = request.Location.DistrictId,
                    TownId = request.Location.TownId,
                    AddressText = request.Location.AddressText
                };
                locationId = await _incidentRepository.CreateLocationAsync(location, ct);
            }
        }

        // Determine status - keep current if not provided, otherwise validate
        var statusId = existing.StatusId;
        if (request.StatusId.HasValue)
        {
            var status = await _lookupRepository.GetStatusByIdAsync(request.StatusId.Value, ct);
            if (status == null)
                throw new ArgumentException("Invalid status");
            statusId = request.StatusId.Value;
        }

        var incident = new IncidentRecord
        {
            Id = id,
            Title = request.Title,
            Description = request.Description,
            SentToUserId = request.SentToUserId,
            CreatedByUserId = existing.CreatedByUserId,
            LocationId = locationId,
            Priority = request.Priority,
            SuggestedActionsTaken = request.SuggestedActionsTaken,
            StatusId = statusId
        };

        await _incidentRepository.UpdateAsync(incident, request.TypeIds, ct);
        
        return (await GetByIdAsync(id, ct))!;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid currentUserId, string userRole, CancellationToken ct = default)
    {
        var existing = await _incidentRepository.GetByIdAsync(id, ct);
        if (existing == null)
            return false;

        // Only admin/officer can delete, or the creator
        var canDelete = userRole == "officer" || userRole == "supervisor" || existing.CreatedByUserId == currentUserId;
        if (!canDelete)
            throw new UnauthorizedAccessException("You don't have permission to delete this incident");

        return await _incidentRepository.DeleteAsync(id, ct);
    }

    public async Task<IncidentResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var incident = await _incidentRepository.GetByIdAsync(id, ct);
        if (incident == null)
            return null;

        return MapToResponse(incident);
    }

    public async Task<PagedResponseDto<IncidentResponseDto>> ListForUserAsync(
        Guid userId, PagedRequestDto request, CancellationToken ct = default)
    {
        var (items, totalCount) = await _incidentRepository.ListForUserAsync(
            userId, request.Page, request.PageSize, ct);

        return new PagedResponseDto<IncidentResponseDto>
        {
            Items = items.Select(MapToResponse).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    private static IncidentResponseDto MapToResponse(IncidentRecord incident)
    {
        return new IncidentResponseDto
        {
            Id = incident.Id,
            Title = incident.Title,
            Description = incident.Description,
            Priority = incident.Priority,
            SuggestedActionsTaken = incident.SuggestedActionsTaken,
            CreatedAt = incident.CreatedAt,
            UpdatedAt = incident.UpdatedAt,
            Status = new IncidentStatusDto
            {
                Id = incident.Status!.Id,
                Code = incident.Status.Code,
                Name = incident.Status.Name,
                NameAr = incident.Status.NameAr
            },
            CreatedByUser = new UserSummaryDto
            {
                Id = incident.CreatedByUser!.Id,
                Username = incident.CreatedByUser.Username,
                RoleName = incident.CreatedByUser.Role?.Name
            },
            SentToUser = incident.SentToUser != null ? new UserSummaryDto
            {
                Id = incident.SentToUser.Id,
                Username = incident.SentToUser.Username,
                RoleName = incident.SentToUser.Role?.Name
            } : null,
            Location = incident.Location != null ? new LocationResponseDto
            {
                Id = incident.Location.Id,
                Lat = incident.Location.Lat,
                Lng = incident.Location.Lng,
                AddressText = incident.Location.AddressText,
                Governorate = incident.Location.Governorate != null ? new GeoLookupDto
                {
                    Id = incident.Location.Governorate.Id,
                    Name = incident.Location.Governorate.Name,
                    NameAr = incident.Location.Governorate.NameAr
                } : null,
                District = incident.Location.District != null ? new GeoLookupDto
                {
                    Id = incident.Location.District.Id,
                    Name = incident.Location.District.Name,
                    NameAr = incident.Location.District.NameAr
                } : null,
                Town = incident.Location.Town != null ? new GeoLookupDto
                {
                    Id = incident.Location.Town.Id,
                    Name = incident.Location.Town.Name,
                    NameAr = incident.Location.Town.NameAr
                } : null
            } : null,
            Types = incident.Types.Select(t => new IncidentTypeDto
            {
                Id = t.Id,
                Name = t.Name,
                NameEn = t.NameEn,
                NameAr = t.NameAr
            }).ToList(),
            AvailableActions = MapWorkflowActions(incident.Status!.Code)
        };
    }

    private static IncidentWorkflowActionsDto MapWorkflowActions(string statusCode)
    {
        var actions = IncidentWorkflowEngine.GetAvailableActions(statusCode);
        return new IncidentWorkflowActionsDto
        {
            CanSendToReview = actions.CanSendToReview,
            CanSendToAccept = actions.CanSendToAccept,
            CanSendToReject = actions.CanSendToReject
        };
    }

    public async Task<IncidentResponseDto> UpdateStatusAsync(Guid id, Guid statusId, Guid currentUserId, CancellationToken ct = default)
    {
        var existing = await _incidentRepository.GetByIdAsync(id, ct);
        if (existing == null)
            throw new KeyNotFoundException("Incident not found");

        // Validate status exists
        var status = await _lookupRepository.GetStatusByIdAsync(statusId, ct);
        if (status == null)
            throw new ArgumentException("Invalid status ID");

        var updated = await _incidentRepository.UpdateStatusAsync(id, statusId, ct);
        if (!updated)
            throw new InvalidOperationException("Failed to update incident status");

        // Call LLM service when status changes to "accepted"
        if (status.Code == IncidentStatus.Accepted)
        {
            await AnalyzeIncidentWithLlmAsync(id, existing.Description, ct);
        }

        return (await GetByIdAsync(id, ct))!;
    }

    private async Task AnalyzeIncidentWithLlmAsync(Guid incidentId, string? description, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(description))
            return;

        try
        {
            var analysisResult = await _llmService.AnalyzeIncidentAsync(description, ct);
            if (analysisResult != null)
            {
                // Update the incident with LLM analysis results
                var incident = await _incidentRepository.GetByIdAsync(incidentId, ct);
                if (incident != null)
                {
                    var updatedIncident = new IncidentRecord
                    {
                        Id = incident.Id,
                        Title = incident.Title,
                        Description = incident.Description,
                        SentToUserId = incident.SentToUserId,
                        CreatedByUserId = incident.CreatedByUserId,
                        LocationId = incident.LocationId,
                        Priority = analysisResult.Severity,
                        SuggestedActionsTaken = analysisResult.SuggestedActionsTaken,
                        StatusId = incident.StatusId
                    };

                    await _incidentRepository.UpdateAsync(updatedIncident, incident.Types.Select(t => t.Id), ct);
                }
            }
        }
        catch (Exception)
        {
            // Log error but don't fail the status update
            // The incident status was already updated successfully
        }
    }
}