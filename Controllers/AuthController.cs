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
    
    // POST /api/auth/password — request password reset (sends email with reset link)
    [HttpPost("password-reset-request")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
    {
        await authService.ForgotPasswordRequestAsync(dto);
        return Ok(new { message = "If an account exists with that email, a reset link has been sent." });
    }

    //POST /api/auth/password — reset password using token from email
    [HttpPost("password-reset")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        await authService.ResetPasswordAsync(dto);
        return Ok(new { message = "Password updated successfully" });
       
    }
    
    // POST /api/auth/logout 
    [HttpPost("logout")]
    public IActionResult Logout()
        => Ok(new { message = "Logged out successfully" });
    
    
}