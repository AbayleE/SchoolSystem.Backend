using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Backend.DTOs.Auth;
using SchoolSystem.Backend.Services.AuthService;

namespace SchoolSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    // POST /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await authService.LoginAsync(dto);
        return Ok(result);
    }


    // POST /api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterWithInvitationDto dto)
    {
        var result = await authService.RegisterWithInvitationAsync(dto);
        return Ok(result);
    }

    // POST /api/auth/logout 
    [HttpPost("logout")]
    public IActionResult Logout()
        => Ok(new { message = "Logged out successfully" });
    
    
}