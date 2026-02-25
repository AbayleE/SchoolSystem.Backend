using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.DTOs.Auth;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services.AuthService;

public class AuthServiceEnhanced(
    IConfiguration config,
    SchoolDbContext context,
    ILogger<AuthServiceEnhanced> logger)
    : IAuthServiceEnhanced
{
    /// <summary>
    /// Verify JWT token validity
    /// </summary>
    public async Task<VerifyTokenResponseDto> VerifyTokenAsync(string token)
    {
        logger.LogInformation("Verifying JWT token");

        try
        {
            var jwtSettings = config.GetSection("Jwt");
            var jwtKey = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not found"));
            var key = new SymmetricSecurityKey(jwtKey);

            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var userIdClaim = principal.FindFirst("UserId")?.Value;
            var tenantIdClaim = principal.FindFirst("TenantId")?.Value;
            var emailClaim = principal.FindFirst("email")?.Value;
            var roleClaim = principal.FindFirst("role")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(emailClaim))
                return new VerifyTokenResponseDto { IsValid = false };

            var user = await context.Users.FindAsync(Guid.Parse(userIdClaim));
            if (user == null)
                return new VerifyTokenResponseDto { IsValid = false };

            return new VerifyTokenResponseDto
            {
                UserId = user.Id,
                TenantId = user.TenantId,
                Email = user.Email ?? "",
                Role = user.Role.ToString(),
                IsValid = true
            };
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Token verification failed");
            return new VerifyTokenResponseDto { IsValid = false };
        }
    }

    /// <summary>
    /// Refresh existing JWT token
    /// </summary>
    public async Task<RefreshTokenResponseDto> RefreshTokenAsync(string existingToken)
    {
        logger.LogInformation("Refreshing JWT token");

        // Verify the existing token is still valid (but may be expired)
        var verifyResult = await VerifyTokenAsync(existingToken);
        if (!verifyResult.IsValid)
            throw new UnauthorizedAccessException("Invalid token");

        // Get the user and generate a new token
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == verifyResult.UserId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        var newToken = GenerateJwtToken(user);
        var expiresInMinutes = int.Parse(config.GetSection("Jwt")["ExpiresInMinutes"] ?? "60");

        return new RefreshTokenResponseDto
        {
            Token = newToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiresInMinutes)
        };
    }

    private string GenerateJwtToken(User user)
    {
        logger.LogInformation("Generating JWT token for user {UserId}", user.Id);

        var jwtSettings = config.GetSection("Jwt");
        var jwtKey = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not found"));
        var key = new SymmetricSecurityKey(jwtKey);

        if (user.Email == null)
            throw new InvalidOperationException("Email not set");

        var claims = new List<System.Security.Claims.Claim>
        {
            new("UserId", user.Id.ToString()),
            new("TenantId", user.TenantId.ToString()),
            new(System.Security.Claims.ClaimTypes.Role, user.Role.ToString()),
            new(System.Security.Claims.ClaimTypes.Email, user.Email)
        };

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresIn = int.Parse(jwtSettings["ExpiresInMinutes"] ?? "60");

        var token = new JwtSecurityToken(
            jwtSettings["Issuer"],
            jwtSettings["Audience"],
            claims,
            expires: DateTime.UtcNow.AddMinutes(expiresIn),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
