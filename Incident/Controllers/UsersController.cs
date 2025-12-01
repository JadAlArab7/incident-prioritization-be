using Incident.DTOs;
using Incident.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Incident.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILookupService _lookupService;

    public UsersController(IUserService userService, ILookupService lookupService)
    {
        _userService = userService;
        _lookupService = lookupService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserSummaryDto>>> GetAll(CancellationToken ct)
    {
        var users = await _userService.GetAllAsync(ct);
        var result = users.Select(u => new UserSummaryDto
        {
            Id = u.Id,
            Username = u.Username,
            RoleName = u.Role?.Name
        });
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserSummaryDto>> GetById(Guid id, CancellationToken ct)
    {
        var user = await _userService.GetByIdAsync(id, ct);
        if (user == null)
            return NotFound();

        return Ok(new UserSummaryDto
        {
            Id = user.Id,
            Username = user.Username,
            RoleName = user.Role?.Name
        });
    }

    [HttpPost]
    [Authorize(Roles = "supervisor,officer")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateUserDto request, CancellationToken ct)
    {
        var id = await _userService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "supervisor,officer")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _userService.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpGet("secretaries")]
    public async Task<ActionResult<IEnumerable<UserSummaryDto>>> GetSecretaries(CancellationToken ct)
    {
        var secretaries = await _lookupService.ListSecretariesAsync(ct);
        return Ok(secretaries);
    }
}