using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.Interface;
using SchoolSystem.Backend.Services;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GradesController(GradeService service, SchoolDbContext context, ITenantContext tenant)
    : BaseController<Grade>(service)
{
    // GET /api/grades/student/{studentId} — all grades for a student
    [Authorize(Roles = "SystemOwner, SchoolAdmin, Manager, Teacher, Student, Parent")]
    [HttpGet("student/{studentId:guid}")]
    public async Task<IActionResult> GetStudentGrades(Guid studentId)
    {
        var grades = await context.Grades
            .Where(g => g.StudentId == studentId && g.TenantId == tenant.TenantId && !g.IsDeleted)
            .Include(g => g.Subject)
            .Include(g => g.Term)
            .Include(g => g.AcademicYear)
            .ToListAsync();
        return Ok(grades);
    }

    // GET /api/grades/student/{studentId}/term/{termId}
    [Authorize(Roles = "SystemOwner, SchoolAdmin, Manager, Teacher, Student, Parent")]
    [HttpGet("student/{studentId:guid}/term/{termId:guid}")]
    public async Task<IActionResult> GetStudentGradesByTerm(Guid studentId, Guid termId)
    {
        var grades = await context.Grades
            .Where(g =>
                g.StudentId == studentId &&
                g.TermId == termId &&
                g.TenantId == tenant.TenantId &&
                !g.IsDeleted)
            .Include(g => g.Subject)
            .ToListAsync();
        return Ok(grades);
    }
}