using SchoolSystem.Backend.Data;

namespace SchoolSystem.Backend.Services;

public class AcademicYearService(SchoolDbContext context)
{
    private readonly SchoolDbContext _context = context;
}