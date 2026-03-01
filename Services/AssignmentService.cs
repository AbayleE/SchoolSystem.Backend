using Microsoft.EntityFrameworkCore;
using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.Interface;
using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services;

public class AssignmentService(
    SchoolDbContext context,
    NotificationService notificationService,
    TenantRepository<Assignment> repo,
    ITenantContext tenant)
    : BaseService<Assignment>(repo)
{

    //Get assignment by id (tenant-scoped via base)
    public new async Task<Assignment?> GetByIdAsync(Guid id) => await base.GetByIdAsync(id);

    public async Task AddSubmission(Guid studentId, Guid assignmentId, Guid fileId)
    {
        var submission = new AssignmentSubmission
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.TenantId,
            AssignmentId = assignmentId,
            StudentId = studentId,
            FileId = fileId,
            SubmittedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.AssignmentSubmissions.Add(submission);
        await context.SaveChangesAsync();
    }

    public async Task<AssignmentSubmission?> GetSubmissionAsync(Guid submissionId)
    {
        return await context.AssignmentSubmissions
            .Where(s => s.Id == submissionId && s.TenantId == tenant.TenantId)
            .FirstOrDefaultAsync();
    }

    public async Task SetSubmissionGrade(Guid submissionId, int score, string? feedback)
    {
        var submission = await GetSubmissionAsync(submissionId);
        if (submission == null)
            throw new InvalidOperationException("Submission not found.");

        submission.Score = score;
        submission.Feedback = feedback;
        submission.GradedAt = DateTime.UtcNow;
        submission.UpdatedAt = DateTime.UtcNow;

        context.AssignmentSubmissions.Update(submission);
        await context.SaveChangesAsync();
    }
    
    public async Task GradeSubmissionAsync (Guid teacherId, Guid submissionId, int score, string feedback)
    {
        var submission = await GetSubmissionAsync(submissionId);

        if (submission == null)
            throw new InvalidOperationException("Submission not found.");

        var assignment = await GetByIdAsync(submission.AssignmentId);
        if (assignment == null || assignment.TeacherId != teacherId)
            throw new UnauthorizedAccessException("You are not authorized to grade this submission.");

        await SetSubmissionGrade(submissionId, score, feedback);

        await notificationService.NotifyStudentAsync(submission.StudentId,
            $"Your assignment has been graded: {score}", feedback);
    }

    public async Task<List<AssignmentSubmission>> GetSubmissionsForAssignmentAsync(Guid assignmentId, Guid teacherId)
    {
        var assignment = await GetByIdAsync(assignmentId);
        if (assignment == null || assignment.TeacherId != teacherId)
            throw new UnauthorizedAccessException("You are not authorized to view submissions for this assignment.");
        
        return await context.AssignmentSubmissions
            .Where(s => s.AssignmentId == assignmentId && s.TenantId == tenant.TenantId)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }
}
