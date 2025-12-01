using Incident.DTOs;
using Incident.Models;
using Incident.Repositories;

namespace Incident.Services;

public class IncidentService : IIncidentService
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IIncidentStatusRepository _statusRepository;
    private readonly ILookupRepository _lookupRepository;

    public IncidentService(
        IIncidentRepository incidentRepository,
        IIncidentStatusRepository statusRepository,
        ILookupRepository lookupRepository)
    {
        _incidentRepository = incidentRepository;
        _statusRepository = statusRepository;
        _lookupRepository = lookupRepository;
    }

    public async Task<IncidentRecord?> GetByIdAsync(Guid id)
    {
        return await _incidentRepository.GetByIdAsync(id);
    }

    public async Task<PagedResponseDto<IncidentRecord>> GetPagedAsync(PagedRequestDto request)
    {
        return await _incidentRepository.GetPagedAsync(request);
    }

    public async Task<IncidentRecord> CreateAsync(IncidentCreateRequestDto request, Guid userId)
    {
        // Get the draft status
        var draftStatus = await _statusRepository.GetByCodeAsync("draft");
        if (draftStatus == null)
        {
            throw new InvalidOperationException("Draft status not found");
        }

        var incident = new IncidentRecord
        {
            Title = request.Title,
            Description = request.Description,
            IncidentTypeId = request.IncidentTypeId,
            LocationId = request.LocationId,
            CreatedByUserId = userId,
            StatusId = draftStatus.Id
        };

        return await _incidentRepository.CreateAsync(incident);
    }

    public async Task<IncidentRecord?> UpdateAsync(Guid id, IncidentUpdateRequestDto request, Guid userId)
    {
        var existingIncident = await _incidentRepository.GetByIdAsync(id);
        if (existingIncident == null)
        {
            return null;
        }

        // Check if user can edit this incident
        var status = await _statusRepository.GetByIdAsync(existingIncident.StatusId);
        if (status == null)
        {
            throw new InvalidOperationException("Status not found");
        }

        bool isCreator = existingIncident.CreatedByUserId == userId;
        bool canEdit = isCreator && (status.Code == "draft" || status.Code == "rejected");

        if (!canEdit)
        {
            throw new UnauthorizedAccessException("You are not authorized to edit this incident");
        }

        existingIncident.Title = request.Title;
        existingIncident.Description = request.Description;
        existingIncident.IncidentTypeId = request.IncidentTypeId;
        existingIncident.LocationId = request.LocationId;

        var updatedIncident = await _incidentRepository.UpdateAsync(existingIncident);
        return updatedIncident;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var existingIncident = await _incidentRepository.GetByIdAsync(id);
        if (existingIncident == null)
        {
            return false;
        }

        // Only creator can delete and only when in draft or rejected status
        bool isCreator = existingIncident.CreatedByUserId == userId;
        if (!isCreator)
        {
            throw new UnauthorizedAccessException("Only the creator can delete this incident");
        }

        var status = await _statusRepository.GetByIdAsync(existingIncident.StatusId);
        if (status == null)
        {
            throw new InvalidOperationException("Status not found");
        }

        bool canDelete = status.Code == "draft" || status.Code == "rejected";
        if (!canDelete)
        {
            throw new UnauthorizedAccessException("Incident can only be deleted when in draft or rejected status");
        }

        return await _incidentRepository.DeleteAsync(id);
    }
}