namespace SchoolSystem.Backend.DTOs.Applications;

public class CreateApplicationDto
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public required string ParentEmail { get; set; }
    public required string ParentPhone { get; set; }
    public string? Parent2Email { get; set; }
    public string? Parent2Phone { get; set; }
    public required string Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public required string GradeApplied { get; set; }
    public Guid AcademicYearId { get; set; }
}
