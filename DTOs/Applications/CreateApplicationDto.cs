using SchoolSystem.Domain.Enums;

namespace SchoolSystem.Backend.DTOs.Applications;

public class CreateApplicationDto
{
    // Student
    public required string FirstName { get; set; }
    public  string? MiddleName { get; set; }
    public required string LastName { get; set; }
    public required DateTime DateOfBirth { get; set; }
    public required Gender Gender { get; set; }
    public required Guid GradeAppliedId { get; set; }
    public string? CurrentGradeLevel { get; set; }
    public decimal? Gpa { get; set; }

    // Address
    public required string Region { get; set; }
    public required string City { get; set; }
    public string? SubCity { get; set; }
    public string? Woreda { get; set; }
    public string? HouseNumber { get; set; }
    
    // Guardians (at least one required)
    public required List<CreateGuardianDto> Guardians { get; set; }
}

public class CreateGuardianDto
{
    public required string FirstName { get; set; }
    
    public string? MiddleName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public required GuardianRelationship Relationship { get; set; }
    public bool IsPrimaryContact { get; set; }
}

