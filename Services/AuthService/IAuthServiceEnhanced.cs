using SchoolSystem.Backend.DTOs.Auth;

namespace SchoolSystem.Backend.Services.AuthService;

public interface IAuthServiceEnhanced
{
    Task<VerifyTokenResponseDto> VerifyTokenAsync(string token);
    Task<RefreshTokenResponseDto> RefreshTokenAsync(string existingToken);
}
