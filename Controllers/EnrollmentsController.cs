using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Backend.Services;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController(EnrollmentService service) : BaseController<Enrollment>(service)
{
    // GET /api/enrollments/check — check if student is in a class
    [Authorize(Roles = "SystemOwner, SchoolAdmin, Manager, Teacher")]
    [HttpGet("check")]
    public async Task<IActionResult> CheckEnrollment(
        [FromQuery] Guid studentId,
        [FromQuery] Guid classId)
    {
        var enrolled = await service.IsStudentInClassAsync(studentId, classId);
        return Ok(new { enrolled });
    }
}