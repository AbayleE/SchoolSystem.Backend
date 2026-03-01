using SchoolSystem.Backend.DTOs.Auth;

namespace SchoolSystem.Backend.Services.AuthService;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<AuthResponseDto> RegisterWithInvitationAsync(RegisterWithInvitationDto dto);
    Task ForgotPasswordRequestAsync(ForgotPasswordRequestDto dto);
    Task ResetPasswordAsync(ResetPasswordDto dto);
}