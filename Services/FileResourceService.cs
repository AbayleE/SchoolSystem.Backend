using SchoolSystem.Backend.Data;

namespace SchoolSystem.Backend.Services;

public class FileResourceService(SchoolDbContext context)
{
    private readonly SchoolDbContext _context = context;
}