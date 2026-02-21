using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Backend.Services.Workflows;
using SchoolSystem.Backend.DTOs;
using Microsoft.AspNetCore.Authorization;

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
            var studentId = User.GetUserId(); // your JWT extension method
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
    }
}