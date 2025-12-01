using Incident.DTOs;
using Incident.Services;
using Microsoft.AspNetCore.Mvc;

namespace Incident.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
    {
        var response = await _authService.AuthenticateAsync(request);
        if (response == null)
        {
            return Unauthorized("Invalid username or password");
        }

        return Ok(response);
    }
}