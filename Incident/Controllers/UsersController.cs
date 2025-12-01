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
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        var result = users.Select(u => new
        {
            u.Id,
            u.Username,
            u.RoleId,
            u.RoleName,
            u.CreatedAt,
            u.UpdatedAt
        });
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { error = "User not found" });
        }

        return Ok(new
        {
            user.Id,
            user.Username,
            user.RoleId,
            user.RoleName,
            user.CreatedAt,
            user.UpdatedAt
        });
    }

    [HttpGet("officers")]
    public async Task<IActionResult> GetOfficers()
    {
        var officers = await _userService.GetOfficersAsync();
        var result = officers.Select(u => new
        {
            u.Id,
            u.Username,
            u.RoleName
        });
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "supervisor")]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest(new { error = "Username and password are required" });
        }

        if (string.IsNullOrWhiteSpace(dto.RoleName))
        {
            return BadRequest(new { error = "Role name is required" });
        }

        try
        {
            var user = await _userService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, new
            {
                user.Id,
                user.Username,
                user.RoleId,
                user.CreatedAt,
                user.UpdatedAt
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "supervisor")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _userService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound(new { error = "User not found" });
        }

        return NoContent();
    }
}