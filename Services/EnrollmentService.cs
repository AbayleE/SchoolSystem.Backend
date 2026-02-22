using Microsoft.EntityFrameworkCore;
using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.Interface;
using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services;

public class EnrollmentService(
    BaseRepository<Enrollment> repo,
    SchoolDbContext context,
    ITenantContext tenant)
    : BaseService<Enrollment>(repo)
{
    private readonly SchoolDbContext _context = context;
    private readonly ITenantContext _tenant = tenant;

    public async Task<bool> IsStudentInClass(Guid studentId, Guid classId)
    {
        return await _context.Enrollments
            .AnyAsync(e => e.StudentId == studentId && e.ClassId == classId && e.TenantId == _tenant.TenantId);
    }
}