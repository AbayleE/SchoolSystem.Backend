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
    private readonly SchoolDbContext _context = context;
    private readonly ITenantContext _tenant = tenant;

    /// <summary>Get assignment by id (tenant-scoped via base).</summary>
    public new async Task<Assignment?> GetByIdAsync(Guid id) => await base.GetByIdAsync(id);

    public async Task AddSubmission(Guid studentId, Guid assignmentId, Guid fileId)
    {
        var submission = new AssignmentSubmission
        {
            Id = Guid.NewGuid(),
            TenantId = _tenant.TenantId,
            AssignmentId = assignmentId,
            StudentId = studentId,
            FileId = fileId,
            SubmittedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.AssignmentSubmissions.Add(submission);
        await _context.SaveChangesAsync();
    }

    public async Task<AssignmentSubmission?> GetSubmission(Guid submissionId)
    {
        return await _context.AssignmentSubmissions
            .Where(s => s.Id == submissionId && s.TenantId == _tenant.TenantId)
            .FirstOrDefaultAsync();
    }

    public async Task<List<AssignmentSubmission>> GetSubmissionsByAssignmentId(Guid assignmentId)
    {
        return await _context.AssignmentSubmissions
            .Where(s => s.AssignmentId == assignmentId && s.TenantId == _tenant.TenantId)
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

        _context.AssignmentSubmissions.Update(submission);
        await _context.SaveChangesAsync();
    }
}
