using SchoolSystem.Domain.ValueObjects;

namespace SchoolSystem.Backend.DTOs.Users;

public class CreateUserDto
{
    public required string TenantId { get; set; }
    public required string FirstName { get; set; } 
    public required string  LastName { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }
    public required string Password { get; set; }
    public required string Role { get; set; }
}