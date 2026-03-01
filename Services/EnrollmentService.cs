using Microsoft.EntityFrameworkCore;
using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.Interface;
using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services;

public class EnrollmentService(
    TenantRepository<Enrollment> repo,
    SchoolDbContext context,
    ITenantContext tenant)
    : TenantService<Enrollment>(repo)
{

    public async Task<bool> IsStudentInClassAsync(Guid studentId, Guid classId)
    {
        return await context.Enrollments
            .AnyAsync(e => e.StudentId == studentId && e.ClassId == classId
                                                    && e.TenantId == tenant.TenantId
                                                    && !e.IsDeleted);
    }
}