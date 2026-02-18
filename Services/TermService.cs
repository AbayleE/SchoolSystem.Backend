using SchoolSystem.Backend.Data;

namespace SchoolSystem.Backend.Services;

public class TermService(SchoolDbContext context)
{
    private readonly SchoolDbContext _context = context;
}