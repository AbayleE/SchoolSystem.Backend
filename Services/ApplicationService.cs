using SchoolSystem.Backend.Data;

namespace SchoolSystem.Backend.Services;

public class ApplicationService(SchoolDbContext context)
{
    private readonly SchoolDbContext _context = context;

    // workflow methods will go here
}