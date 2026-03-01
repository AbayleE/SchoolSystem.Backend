using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Backend.DTOs.Grades;
using SchoolSystem.Backend.Extensions;
using SchoolSystem.Backend.Services;
using SchoolSystem.Domain.Enums;

namespace SchoolSystem.Backend.Controllers;

[ApiController]
[Route("api/assignments")]
public class AssignmentController(AssignmentService service, FileResourceService fileService) : ControllerBase
{
    // POST /api/assignments/{assignmentId}/submissions
    [Authorize(Roles = "Student")]
    [HttpPost("{assignmentId:guid}/submissions")]
    public async Task<IActionResult> SubmitAssignment(Guid assignmentId, IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file provided." });
        
        var studentId = User.GetUserId();

        var resource = await fileService.UploadAsync(file, FileType.AssignmentSubmission,assignmentId, nameof(Domain.Entities.Assignment));
        await service.AddSubmission(studentId, assignmentId, resource.Id);

        return Ok(new { message = "Assignment submitted successfully." });
    }

    // POST /api/assignments/submissions/{submissionId}/grade
    [Authorize(Roles = "Teacher")]
    [HttpPost("submissions/{submissionId:guid}/grade")]
    public async Task<IActionResult> GradeSubmission(
        Guid submissionId,
        [FromBody] GradeSubmissionDto dto)
    {
        var teacherId = User.GetUserId();
        await service.GradeSubmissionAsync(teacherId, submissionId, dto.Score, dto.Feedback!);
        return Ok(new { message = "Submission graded successfully." });
    }

    // GET /api/assignments/{assignmentId}
    [Authorize(Roles = "Student, Teacher, SchoolAdmin , Manager")]
    [HttpGet("{assignmentId:guid}")]
    public async Task<IActionResult> GetAssignment(Guid assignmentId)
    {
        var assignment = await service.GetByIdAsync(assignmentId);
        return assignment == null ? NotFound() : Ok(assignment);
    }

    // GET /api/assignments/{assignmentId}/submissions
    [Authorize(Roles = "Teacher")]
    [HttpGet("{assignmentId:guid}/submissions")]
    public async Task<IActionResult> GetSubmissions(Guid assignmentId)
    {
        var teacherId = User.GetUserId();
        var submissions = await service.GetSubmissionsForAssignmentAsync(assignmentId, teacherId);
        return Ok(submissions);
    }

    // GET /api/assignments/submissions/{submissionId}
    [Authorize(Roles = "Teacher,Student")]
    [HttpGet("submissions/{submissionId:guid}")]
    public async Task<IActionResult> GetSubmission(Guid submissionId)
    {
        var requesterId = User.GetUserId();
        var requesterRole = User.GetUserRole();

        var submission = await service.GetSubmissionAsync(submissionId);
        
        if(submission == null)
            return NotFound();

        // Students can only view their own submissions
        if (requesterRole == "Student" && submission.StudentId != requesterId)
            return Forbid();

        return Ok(submission);
    }
}
