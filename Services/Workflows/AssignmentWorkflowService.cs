using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services.Workflows;

public class AssignmentWorkflowService(
    AssignmentService assignmentService,
    FileResourceService fileService,
    NotificationService notificationService,
    EnrollmentService enrollmentService)
{

    // ---------------------------------------------------------
    // Student submits assignment
    // ---------------------------------------------------------
    public async Task SubmitAssignment(Guid studentId, Guid assignmentId, IFormFile file)
    {
        var assignment = await assignmentService.GetByIdAsync(assignmentId);

        if (assignment == null)
            throw new InvalidOperationException("Assignment not found.");

        var isEnrolled = await enrollmentService.IsStudentInClass(studentId, assignment.ClassId);
        if (!isEnrolled)
            throw new InvalidOperationException("Student is not enrolled in this class.");

        var fileResource = await fileService.UploadAsync(file);

        await assignmentService.AddSubmission(studentId, assignmentId, fileResource.Id);

        await notificationService.NotifyTeacher(
            assignment.TeacherId,
            $"A student submitted work for: {assignment.Title}"
        );
    }

    // ---------------------------------------------------------
    // Teacher grades submission
    // ---------------------------------------------------------
    public async Task GradeSubmission(Guid teacherId, Guid submissionId, int score, string? feedback)
    {
        var submission = await assignmentService.GetSubmission(submissionId);

        if (submission == null)
            throw new InvalidOperationException("Submission not found.");

        var assignment = await assignmentService.GetByIdAsync(submission.AssignmentId);
        if (assignment == null || assignment.TeacherId != teacherId)
            throw new UnauthorizedAccessException("You are not authorized to grade this submission.");

        await assignmentService.SetSubmissionGrade(submissionId, score, feedback);

        await notificationService.NotifyStudent(
            submission.StudentId,
            $"Your assignment has been graded: {score}"
        );
    }

    // ---------------------------------------------------------
    // Get assignment (for workflow context)
    // ---------------------------------------------------------
    public async Task<Assignment?> GetAssignmentAsync(Guid assignmentId) =>
        await assignmentService.GetByIdAsync(assignmentId);

    // ---------------------------------------------------------
    // Get submission (for workflow context)
    // ---------------------------------------------------------
    public async Task<AssignmentSubmission?> GetSubmissionAsync(Guid submissionId) =>
        await assignmentService.GetSubmission(submissionId);

    // ---------------------------------------------------------
    // List submissions for an assignment (teacher)
    // ---------------------------------------------------------
    public async Task<List<AssignmentSubmission>> GetSubmissionsForAssignmentAsync(Guid assignmentId, Guid teacherId)
    {
        var assignment = await assignmentService.GetByIdAsync(assignmentId);
        if (assignment == null)
            throw new InvalidOperationException("Assignment not found.");
        if (assignment.TeacherId != teacherId)
            throw new UnauthorizedAccessException("You are not authorized to view submissions for this assignment.");
        return await assignmentService.GetSubmissionsByAssignmentId(assignmentId);
    }
}