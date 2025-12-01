using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Incident.DTOs;
using Incident.Services;

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
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        var userDtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
            Role = u.Role?.Name ?? "user",
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        });

        return Ok(userDtos);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetUserById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { error = "User not found" });
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role?.Name ?? "user",
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        return Ok(userDto);
    }

    [HttpGet("officers")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetOfficers()
    {
        var officers = await _userService.GetUsersByRoleAsync("officer");
        var userDtos = officers.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
            Role = u.Role?.Name ?? "officer",
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        });

        return Ok(userDtos);
    }

    [HttpPost]
    [Authorize(Roles = "supervisor")]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        if (string.IsNullOrWhiteSpace(createUserDto.Username))
        {
            return BadRequest(new { error = "Username is required" });
        }

        if (string.IsNullOrWhiteSpace(createUserDto.Password))
        {
            return BadRequest(new { error = "Password is required" });
        }

        var existingUser = await _userService.GetUserByUsernameAsync(createUserDto.Username);
        if (existingUser != null)
        {
            return Conflict(new { error = "Username already exists" });
        }

        var user = await _userService.CreateUserAsync(createUserDto);
        if (user == null)
        {
            return BadRequest(new { error = "Failed to create user" });
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role?.Name ?? "user",
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, userDto);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "supervisor")]
    public async Task<ActionResult> DeleteUser(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { error = "User not found" });
        }

        var result = await _userService.DeleteUserAsync(id);
        if (!result)
        {
            return BadRequest(new { error = "Failed to delete user" });
        }

        return NoContent();
    }
}