using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.DTOs.Auth;
using SchoolSystem.Backend.DTOs.Emails;
using SchoolSystem.Backend.Exceptions;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Enums;
using SchoolSystem.Domain.ValueObjects;

namespace SchoolSystem.Backend.Services.AuthService;

public class AuthService(
    IConfiguration config,
    SchoolDbContext context,
    ILogger<AuthService> logger,
    EmailService emailService,
    UserService userService,
    PasswordHasher<User> passwordHasher) : IAuthService
{
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email && !u.IsDeleted) ?? throw new InvalidCredentialsException("Invalid credentials");

        if (!user.IsActive)
            throw new InvalidCredentialsException("This account has been deactivated");
  
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash ?? throw new InvalidOperationException(), dto.Password);

        if (result == PasswordVerificationResult.Failed)
            throw new InvalidCredentialsException("Invalid credentials");
        
        logger.LogInformation("User {UserId} logged in", user.Id);
        return BuildAuthResponse(user);
    }

    public async Task<AuthResponseDto> RegisterWithInvitationAsync(RegisterWithInvitationDto dto)
    {
        var invitation = await context.Invitations
            .FirstOrDefaultAsync(i => i.Token == dto.Token && !i.Used && i.ExpiresAt > DateTime.UtcNow) 
                         ?? throw new InvalidInvitationException("Invalid or expired invitation.");

        var alreadyExists = await context.Users
            .AnyAsync(u => u.Email == invitation.Email && !u.IsDeleted);
        if (alreadyExists)
            throw new InvalidInvitationException("An account with this email already exists.");
        

        var user = new User
        {
            TenantId = invitation.TenantId,
            Name = new FullName(dto.FirstName, dto.MiddleName ?? "", dto.LastName),
            Email = invitation.Email,
            Role = invitation.Role,
            Phone = dto.Phone,
            IsActive = true
        };

        user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);
        context.Users.Add(user);
        
        userService.CreateRoleProfile(user);
       
        invitation.Used = true;
        await context.SaveChangesAsync();

        logger.LogInformation("User {UserId} registered via invitation with role {Role}", user.Id, user.Role);
        return BuildAuthResponse(user);
    }
    
    public async Task ForgotPasswordRequestAsync (ForgotPasswordRequestDto dto)
    {
        var tenantId = await context.Tenants.Where(tenant => tenant.Name == dto.SchoolName).Select(tenant => tenant.Id).FirstAsync();
        if(tenantId == Guid.Empty)
            throw new NotFoundException("Tenant not found.");
      
        var user = await context.Users.Where(user => user.Email == dto.Email && !user.IsDeleted && user.TenantId == tenantId ).FirstOrDefaultAsync();
      
        if(user == null)
            throw new NotFoundException("User not found.");
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        user.PasswordResetToken = token;
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
        await context.SaveChangesAsync();

        var passwordResetLink = $"{config["App:BaseUrl"]}/pages/reset-password.html?token={Uri.EscapeDataString(token)}";
        
        var emailMessage = new EmailMessageDto
        {
            To = user.Email,
            Subject = "Reset Password Request",
            HtmlBody = $"""
                        <h2>Password Reset Request</h2>
                        <p>We received a request to reset your password. Click the button below to reset it. This link expires in <strong>1 hour</strong>.</p>
                        <br>
                        <a href="{passwordResetLink}" 
                           style="background:#4F46E5;color:white;padding:12px 24px;border-radius:6px;text-decoration:none;">
                           Reset Password
                        </a>
                        <p>Or copy this link into your browser:</p>
                        <p>{passwordResetLink}</p>
                        <p>If you didn't request a password reset, you can safely ignore this email.</p>
                        """,
            TextBody = $"""
                        We received a request to reset your password. Use this link to reset it (expires in 1 hour):
                        {passwordResetLink}

                        If you didn't request this, you can ignore this email.
                        """
        };
        await emailService.SendEmailInternal(emailMessage);
            logger.LogInformation("Password reset email sent to {Email}", user.Email);
    }

    public async Task ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await context.Users
            .Where(u => u.PasswordResetToken == dto.Token
                        && u.PasswordResetTokenExpiry > DateTime.UtcNow
                        && !u.IsDeleted)
            .FirstOrDefaultAsync();

        if (user == null)
            throw new NotFoundException("Invalid or expired reset token.");

        user.PasswordHash = passwordHasher.HashPassword(user, dto.NewPassword);
        user.PasswordResetToken = null;      
        user.PasswordResetTokenExpiry = null;
        
        await context.SaveChangesAsync();
        logger.LogInformation("User {UserId} password updated", user.Id);
    }
    

    private AuthResponseDto BuildAuthResponse(User user)
    {
        return new AuthResponseDto
        {
            UserId = user.Id,
            TenantId = user.TenantId,
            Role = user.Role,
            Email = user.Email!,
            Token = GenerateJwtToken(user)
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
            new(ClaimTypes.Email, user.Email!)
        };
        
        var expiresIn = int.Parse(jwtSettings["ExpiresInMinutes"] ?? "60");

        var token = new JwtSecurityToken(
            jwtSettings["Issuer"],
            jwtSettings["Audience"],
            claims,
            expires: DateTime.UtcNow.AddMinutes(expiresIn),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );
        
        logger.LogInformation("JWT token generated for user {UserId}", user.Id);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    
    /*  /// <summary>
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
*/
}