using SchoolSystem.Domain.valueObject;

namespace SchoolSystem.Backend.DTOs.Users;

public class CreateUserDto
{
    public Guid TenantId { get; set; }
    public FullName Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}