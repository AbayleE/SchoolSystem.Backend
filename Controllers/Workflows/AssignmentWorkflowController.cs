using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Backend.Extensions;
using SchoolSystem.Backend.Services.Workflows;
using SchoolSystem.Backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using SchoolSystem.Backend.DTOs.Grades;

namespace SchoolSystem.Backend.Controllers.Workflows
{
    [ApiController]
    [Route("api/workflows/assignments")]
    public class AssignmentWorkflowController : ControllerBase
    {
        private readonly AssignmentWorkflowService _workflow;

        public AssignmentWorkflowController(AssignmentWorkflowService workflow)
        {
            _workflow = workflow;
        }

        // ---------------------------------------------------------
        // Student submits assignment
        // ---------------------------------------------------------
        [Authorize(Roles = "Student")]
        [HttpPost("{assignmentId}/submit")]
        public async Task<IActionResult> SubmitAssignment(Guid assignmentId, IFormFile file)
        {
            var studentId = User.GetUserId();
            await _workflow.SubmitAssignment(studentId, assignmentId, file);
            return Ok(new { message = "Assignment submitted successfully" });
        }

        // ---------------------------------------------------------
        // Teacher grades submission
        // ---------------------------------------------------------
        [Authorize(Roles = "Teacher")]
        [HttpPost("submissions/{submissionId}/grade")]
        public async Task<IActionResult> GradeSubmission(Guid submissionId, [FromBody] GradeSubmissionDto dto)
        {
            var teacherId = User.GetUserId();
            await _workflow.GradeSubmission(teacherId, submissionId, dto.Score, dto.Feedback);
            return Ok(new { message = "Submission graded successfully" });
        }

        // ---------------------------------------------------------
        // Get assignment (Student / Teacher)
        // ---------------------------------------------------------
        [Authorize(Roles = "Student, Teacher")]
        [HttpGet("{assignmentId}")]
        public async Task<IActionResult> GetAssignment(Guid assignmentId)
        {
            var assignment = await _workflow.GetAssignmentAsync(assignmentId);
            return assignment == null ? NotFound() : Ok(assignment);
        }

        // ---------------------------------------------------------
        // List submissions for an assignment (Teacher)
        // ---------------------------------------------------------
        [Authorize(Roles = "Teacher")]
        [HttpGet("{assignmentId}/submissions")]
        public async Task<IActionResult> GetSubmissions(Guid assignmentId)
        {
            var teacherId = User.GetUserId();
            var submissions = await _workflow.GetSubmissionsForAssignmentAsync(assignmentId, teacherId);
            return Ok(submissions);
        }

        // ---------------------------------------------------------
        // Get single submission (Teacher)
        // ---------------------------------------------------------
        [Authorize(Roles = "Teacher")]
        [HttpGet("submissions/{submissionId}")]
        public async Task<IActionResult> GetSubmission(Guid submissionId)
        {
            var submission = await _workflow.GetSubmissionAsync(submissionId);
            return submission == null ? NotFound() : Ok(submission);
        }
    }
}