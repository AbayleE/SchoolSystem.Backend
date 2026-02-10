using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Backend.DTOs.Auth;
using SchoolSystem.Backend.Services.AuthService;

namespace SchoolSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await authService.LoginAsync(dto);
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterWithInvitationDto dto)
    {
        var result = await authService.RegisterWithInvitationAsync(dto);
        return Ok(result);
    }
}