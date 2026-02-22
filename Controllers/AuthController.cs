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
        var (user, token) = await authService.LoginAsync(dto);
        return Ok(new
        {
            user = new {
                id = user.Id,
                tenantId = user.TenantId,
                role = user.Role.ToString()  
            },
            token = token
        });
}

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterWithInvitationDto dto)
    {
        var (user, token) = await authService.RegisterWithInvitationAsync(dto);
        return Ok(new
        {
            user = new {
                id = user.Id,
                tenantId = user.TenantId,
                role = user.Role.ToString(),
                email = user.Email,
            },
            token = token
        });
    }
}