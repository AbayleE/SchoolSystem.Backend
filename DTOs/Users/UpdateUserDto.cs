using SchoolSystem.Domain.ValueObjects;

namespace SchoolSystem.Backend.DTOs.Users;

public class UpdateUserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    
    public string? Region { get; set; }
    public string? City { get; set; }
    
    public string? SubCity { get; set; }
    
    public string ? Woreda { get; set; }
    public string? HouseNumber { get; set; }
}
