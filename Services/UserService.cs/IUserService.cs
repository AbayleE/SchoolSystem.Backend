using SchoolSystem.Backend.DTOs.Users;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services.UserService.cs;

public interface IUserService
{
    Task<User> CreateUserAsync(CreateUserDto dto);
    Task<User?> GetUserByIdAsync(Guid id);
    Task<List<User>> GetUsersByTenantAsync(Guid tenantId);
}