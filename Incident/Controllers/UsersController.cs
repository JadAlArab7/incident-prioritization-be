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
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        var userDtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            FullName = u.FullName,
            Role = u.Role?.Code ?? "user"
        });

        return Ok(userDtos);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role?.Code ?? "user"
        };

        return Ok(userDto);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<UserDto>> Create(CreateUserDto createUserDto)
    {
        try
        {
            var user = await _userService.CreateAsync(createUserDto);
            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role?.Code ?? "user"
            };

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, userDto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<UserDto>> Update(Guid id, CreateUserDto updateUserDto)
    {
        try
        {
            var user = await _userService.UpdateAsync(id, updateUserDto);
            if (user == null)
            {
                return NotFound();
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role?.Code ?? "user"
            };

            return Ok(userDto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _userService.DeleteAsync(id);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}