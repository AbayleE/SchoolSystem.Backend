using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.DTOs.Auth;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.valueObject;

namespace SchoolSystem.Backend.Services.AuthService;

public class AuthService(
    IConfiguration config,
    SchoolDbContext context,
    ILogger<AuthService> logger,
    PasswordHasher<User> passwordHasher) : IAuthService
{
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null)
            throw new Exception("Invalid credentials");

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

        if (result == PasswordVerificationResult.Failed)
            throw new Exception("Invalid credentials");

        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            Role = user.Role,
            TenantId = user.TenantId
        };
    }

    public async Task<AuthResponseDto> RegisterWithInvitationAsync(RegisterWithInvitationDto dto)
    {
        var invitation = await context.Invitations
            .FirstOrDefaultAsync(i => i.Token == dto.Token && !i.Used && i.ExpiresAt > DateTime.UtcNow);

        if (invitation == null)
            throw new InvalidOperationException("Invalid or expired invitation token.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = invitation.TenantId,
            Name = new FullName(dto.FirstName, dto.MiddleName!, dto.LastName),
            Email = invitation.Email,
            Role = invitation.Role,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);

        context.Users.Add(user);
        invitation.Used = true;

        await context.SaveChangesAsync();

        var token = GenerateJwtToken(user);
        return new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            Role = user.Role,
            TenantId = user.TenantId
        };
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = config.GetSection("Jwt");
        var jwtKey =
            Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not found"));
        var key = new SymmetricSecurityKey(jwtKey);

        var claims = new List<Claim>
        {
            new("UserId", user.Id.ToString()),
            new("TenantId", user.TenantId.ToString()),
            new(ClaimTypes.Role, user.Role.ToString()),
            new(ClaimTypes.Email, user.Email)
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