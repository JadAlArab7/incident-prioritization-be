using Incident.DTOs;
using Incident.Models;
using Incident.Repositories;

namespace Incident.Services;

public class IncidentService : IIncidentService
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IIncidentStatusRepository _statusRepository;

    public IncidentService(
        IIncidentRepository incidentRepository,
        IIncidentStatusRepository statusRepository)
    {
        _incidentRepository = incidentRepository;
        _statusRepository = statusRepository;
    }

    public async Task<IncidentResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var incident = await _incidentRepository.GetByIdAsync(id, ct);
        if (incident == null)
        {
            return null;
        }

        return MapToResponseDto(incident);
    }

    public async Task<PagedResponseDto<IncidentResponseDto>> GetAllAsync(PagedRequestDto request, CancellationToken ct = default)
    {
        var incidents = await _incidentRepository.GetAllAsync(request.Page, request.PageSize, ct);
        var totalCount = await _incidentRepository.GetTotalCountAsync(ct);

        return new PagedResponseDto<IncidentResponseDto>
        {
            Items = incidents.Select(MapToResponseDto).ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<IncidentResponseDto> CreateAsync(IncidentCreateRequestDto request, Guid createdByUserId, CancellationToken ct = default)
    {
        // Get default status (draft)
        var draftStatusId = await _statusRepository.GetStatusIdByCodeAsync("draft", ct);
        if (draftStatusId == null)
        {
            throw new InvalidOperationException("Draft status not found in database");
        }

        var incident = new IncidentRecord
        {
            Title = request.Title,
            Description = request.Description,
            IncidentTypeId = request.IncidentTypeId,
            StatusId = draftStatusId.Value,
            LocationId = request.LocationId,
            CreatedByUserId = createdByUserId
        };

        var id = await _incidentRepository.CreateAsync(incident, ct);
        var created = await _incidentRepository.GetByIdAsync(id, ct);

        return MapToResponseDto(created!);
    }

    public async Task<IncidentResponseDto?> UpdateAsync(Guid id, IncidentUpdateRequestDto request, CancellationToken ct = default)
    {
        var incident = await _incidentRepository.GetByIdAsync(id, ct);
        if (incident == null)
        {
            return null;
        }

        incident.Title = request.Title ?? incident.Title;
        incident.Description = request.Description ?? incident.Description;
        incident.IncidentTypeId = request.IncidentTypeId ?? incident.IncidentTypeId;
        incident.LocationId = request.LocationId ?? incident.LocationId;

        await _incidentRepository.UpdateAsync(incident, ct);
        var updated = await _incidentRepository.GetByIdAsync(id, ct);

        return MapToResponseDto(updated!);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        return await _incidentRepository.DeleteAsync(id, ct);
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