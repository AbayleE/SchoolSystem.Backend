using SchoolSystem.Domain.Enums;

namespace SchoolSystem.Backend.DTOs.Applications;

public class ApplicationFilterDto
{
    public ApplicationStatus? Status { get; set; }
    public Guid? AcademicYearId { get; set; }
    public string? SortBy { get; set; }
    public bool Descending { get; set; } = true;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
