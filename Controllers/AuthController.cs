using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Backend.DTOs.Auth;
using SchoolSystem.Backend.Services.AuthService;

namespace SchoolSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, IAuthServiceEnhanced authServiceEnhanced) : ControllerBase
{
    // ---------------------------------------------------------
    // POST /api/auth/login
    // Accept email & password, hash password check, generate JWT token
    // ---------------------------------------------------------
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

    // ---------------------------------------------------------
    // POST /api/auth/register
    // Register with invitation token
    // ---------------------------------------------------------
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

    // ---------------------------------------------------------
    // POST /api/auth/logout
    // Clear session/token (client-side handling - just return success)
    // ---------------------------------------------------------
    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // Token-based auth doesn't require server-side logout
        // Client should discard the token
        return Ok(new { message = "Logged out successfully" });
    }

    // ---------------------------------------------------------
    // GET /api/auth/verify
    // Validate JWT signature, check expiration, return user info
    // ---------------------------------------------------------
    [Authorize]
    [HttpGet("verify")]
    public async Task<IActionResult> VerifyToken()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var result = await authServiceEnhanced.VerifyTokenAsync(token);

        if (!result.IsValid)
            return Unauthorized(new { message = "Invalid or expired token" });

        return Ok(result);
    }

    // ---------------------------------------------------------
    // POST /api/auth/refresh-token
    // Validate existing token, generate new token
    // ---------------------------------------------------------
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenDto dto)
    {
        try
        {
            var result = await authServiceEnhanced.RefreshTokenAsync(dto.Token);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // ---------------------------------------------------------
    // GET /api/auth/validate-invitation
    // Check invitation code validity and return school & role info
    // ---------------------------------------------------------
    [HttpGet("validate-invitation/{token}")]
    public async Task<IActionResult> ValidateInvitation(string token)
    {
        // This endpoint is handled by InvitationsController GET /api/invitations/token/{token}
        return Ok(new { message = "Use GET /api/invitations/token/{token} instead" });
    }
}