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

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var users = await _userService.GetAllAsync(ct);
        return Ok(users.Select(u => new
        {
            u.Id,
            u.Username,
            u.Email,
            u.FullName,
            u.RoleId,
            u.RoleName,
            u.IsActive,
            u.CreatedAt
        }));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var user = await _userService.GetByIdAsync(id, ct);

        if (user == null)
        {
            return NotFound(new { error = $"User with ID {id} not found" });
        }

        return Ok(new
        {
            user.Id,
            user.Username,
            user.Email,
            user.FullName,
            user.RoleId,
            user.RoleName,
            user.IsActive,
            user.CreatedAt
        });
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create([FromBody] CreateUserDto request, CancellationToken ct)
    {
        var id = await _userService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }
}