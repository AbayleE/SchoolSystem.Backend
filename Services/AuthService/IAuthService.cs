using SchoolSystem.Backend.DTOs.Auth;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services.AuthService;

public interface IAuthService
{
    Task<(User, string)> LoginAsync(LoginDto dto);
    Task<(User, string)> RegisterWithInvitationAsync(RegisterWithInvitationDto dto);
}