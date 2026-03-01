using SchoolSystem.Domain.ValueObjects;

namespace SchoolSystem.Backend.DTOs.Users;

public  class CreateAdminUserDto
{
    public required string Tenant { get; set; }
    public required string FirstName { get; set; }
    public  string? MiddleName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }
    public required string Password { get; set; }
}