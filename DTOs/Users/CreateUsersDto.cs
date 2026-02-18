using SchoolSystem.Domain.ValueObjects;

namespace SchoolSystem.Backend.DTOs.Users;

public abstract class CreateUserDto
{
    public required Guid TenantId { get; set; }
    public required FullName Name { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }
    public required string Password { get; set; }
    public required string Role { get; set; }
}