using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services.Workflows;

public class AssignmentWorkflowService(
    AssignmentService assignmentService,
    FileResourceService fileService,
    NotificationService notificationService,
    EnrollmentService enrollmentService)
{
    private readonly AssignmentService _assignmentService = assignmentService;
    private readonly FileResourceService _fileService = fileService;
    private readonly NotificationService _notificationService = notificationService;
    private readonly EnrollmentService _enrollmentService = enrollmentService;

    // ---------------------------------------------------------
    // Student submits assignment
    // ---------------------------------------------------------
    public async Task SubmitAssignment(Guid studentId, Guid assignmentId, IFormFile file)
    {
        var assignment = await _assignmentService.GetByIdAsync(assignmentId);

        if (assignment == null)
            throw new InvalidOperationException("Assignment not found.");

        var isEnrolled = await _enrollmentService.IsStudentInClass(studentId, assignment.ClassId);
        if (!isEnrolled)
            throw new InvalidOperationException("Student is not enrolled in this class.");

        var fileResource = await _fileService.UploadAsync(file);

        await _assignmentService.AddSubmission(studentId, assignmentId, fileResource.Id);

        await _notificationService.NotifyTeacher(
            assignment.TeacherId,
            $"A student submitted work for: {assignment.Title}"
        );
    }

    // ---------------------------------------------------------
    // Teacher grades submission
    // ---------------------------------------------------------
    public async Task GradeSubmission(Guid teacherId, Guid submissionId, int score, string? feedback)
    {
        var submission = await _assignmentService.GetSubmission(submissionId);

        if (submission == null)
            throw new InvalidOperationException("Submission not found.");

        var assignment = await _assignmentService.GetByIdAsync(submission.AssignmentId);
        if (assignment == null || assignment.TeacherId != teacherId)
            throw new UnauthorizedAccessException("You are not authorized to grade this submission.");

        await _assignmentService.SetSubmissionGrade(submissionId, score, feedback);

        await _notificationService.NotifyStudent(
            submission.StudentId,
            $"Your assignment has been graded: {score}"
        );
    }

    // ---------------------------------------------------------
    // Get assignment (for workflow context)
    // ---------------------------------------------------------
    public async Task<Assignment?> GetAssignmentAsync(Guid assignmentId) =>
        await _assignmentService.GetByIdAsync(assignmentId);

    // ---------------------------------------------------------
    // Get submission (for workflow context)
    // ---------------------------------------------------------
    public async Task<AssignmentSubmission?> GetSubmissionAsync(Guid submissionId) =>
        await _assignmentService.GetSubmission(submissionId);

    // ---------------------------------------------------------
    // List submissions for an assignment (teacher)
    // ---------------------------------------------------------
    public async Task<List<AssignmentSubmission>> GetSubmissionsForAssignmentAsync(Guid assignmentId, Guid teacherId)
    {
        var assignment = await _assignmentService.GetByIdAsync(assignmentId);
        if (assignment == null)
            throw new InvalidOperationException("Assignment not found.");
        if (assignment.TeacherId != teacherId)
            throw new UnauthorizedAccessException("You are not authorized to view submissions for this assignment.");
        return await _assignmentService.GetSubmissionsByAssignmentId(assignmentId);
    }
}