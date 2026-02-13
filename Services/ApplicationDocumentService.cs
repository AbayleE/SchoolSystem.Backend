using SchoolSystem.Backend.Data;

namespace SchoolSystem.Backend.Services;

public class ApplicationDocumentService(SchoolDbContext context)
{
    private readonly SchoolDbContext _context = context;
}