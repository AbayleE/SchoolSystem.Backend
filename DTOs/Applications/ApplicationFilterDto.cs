namespace SchoolSystem.Backend.DTOs.Applications;

public class ApplicationFilterDto
{
    public string? Status { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "SubmittedAt";
    public bool Descending { get; set; } = true;
}
