using SchoolSystem.Domain.Enums;

namespace SchoolSystem.Backend.DTOs.Applications;

public class ReviewApplicationDto
{
    public required ApplicationStatus Status { get; set; }  // typed, not string
    public string? Notes { get; set; }
}