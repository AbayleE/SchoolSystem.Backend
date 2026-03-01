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
public class StudentsController(StudentService service, SchoolDbContext context, ITenantContext tenant) : BaseController<Student>(service)
{
    // GET /api/students/{id}/parents — get parents linked to a student
    [Authorize(Roles = "SystemOwner, SchoolAdmin, Manager, Teacher")]
    [HttpGet("{id:guid}/parents")]
    public async Task<IActionResult> GetStudentParents(Guid id)
    {
        var parents = await context.StudentParents
            .Where(sp => sp.StudentId == id && !sp.IsDeleted && sp.TenantId == tenant.TenantId)
            .Include(sp => sp.Parent)
            .ThenInclude(p => p!.User)
            .ToListAsync();
        return Ok(parents);
    }

    // GET /api/students/{id}/enrollments
    [Authorize(Roles = "SystemOwner, SchoolAdmin, Manager, Teacher, Parent")]
    [HttpGet("{id:guid}/enrollments")]
    public async Task<IActionResult> GetStudentEnrollments(Guid id)
    {
        var enrollments = await context.Enrollments
            .Where(e => e.StudentId == id && e.TenantId == tenant.TenantId && !e.IsDeleted)
            .Include(e => e.Class)
            .Include(e => e.AcademicYear)
            .ToListAsync();
        return Ok(enrollments);
    }
}