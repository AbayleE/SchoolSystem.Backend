using SchoolSystem.Backend.Data;

namespace SchoolSystem.Backend.Services;

public class TranscriptRequestService(SchoolDbContext context)
{
    private readonly SchoolDbContext _context = context;
}