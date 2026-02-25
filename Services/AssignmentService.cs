using Microsoft.EntityFrameworkCore;
using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.Interface;
using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services;

public class AssignmentService(
    SchoolDbContext context,
    BaseRepository<Assignment> repo,
    ITenantContext tenant)
    : BaseService<Assignment>(repo)
{

    /// <summary>Get assignment by id (tenant-scoped via base).</summary>
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

    public async Task<AssignmentSubmission?> GetSubmission(Guid submissionId)
    {
        return await context.AssignmentSubmissions
            .Where(s => s.Id == submissionId && s.TenantId == tenant.TenantId)
            .FirstOrDefaultAsync();
    }

    public async Task<List<AssignmentSubmission>> GetSubmissionsByAssignmentId(Guid assignmentId)
    {
        return await context.AssignmentSubmissions
            .Where(s => s.AssignmentId == assignmentId && s.TenantId == tenant.TenantId)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }

    public async Task SetSubmissionGrade(Guid submissionId, int score, string? feedback)
    {
        var submission = await GetSubmission(submissionId);
        if (submission == null)
            throw new InvalidOperationException("Submission not found.");

        submission.Score = score;
        submission.Feedback = feedback;
        submission.GradedAt = DateTime.UtcNow;
        submission.UpdatedAt = DateTime.UtcNow;

        context.AssignmentSubmissions.Update(submission);
        await context.SaveChangesAsync();
    }
}
