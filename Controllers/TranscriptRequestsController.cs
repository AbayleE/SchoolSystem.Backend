using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.DTOs.TranscriptRequest;
using SchoolSystem.Backend.Extensions;
using SchoolSystem.Backend.Interface;
using SchoolSystem.Backend.Services;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Enums;

namespace SchoolSystem.Backend.Controllers;
[ApiController]
[Route("api/[controller]")]
public class TranscriptRequestsController(TranscriptRequestService service, SchoolDbContext context, ITenantContext tenant)
    : BaseController<TranscriptRequest>(service)
{
    // POST /api/transcriptrequests — student requests a transcript
    [Authorize(Roles = "Student")]
    [HttpPost("request")]
    public async Task<IActionResult> RequestTranscript([FromBody] CreateTranscriptRequestDto dto)
    {
        var studentId = User.GetUserId();
        var request = new TranscriptRequest
        {
            TenantId = tenant.TenantId,
            StudentId = studentId,
            AcademicYearId = dto.AcademicYearId,
            Status = TranscriptStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };
        var created = await service.AddAsync(request);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    // PUT /api/transcriptrequests/{id}/complete — admin marks as complete
    [Authorize(Roles = "SystemOwner, SchoolAdmin, Manager")]
    [HttpPut("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id)
    {
        var request = await service.GetByIdAsync(id)
                      ?? throw new NotFoundException("Transcript request not found.");

        request.Status = TranscriptStatus.Completed;
        request.CompletedAt = DateTime.UtcNow;
        await service.UpdateAsync(request);
        return Ok(new { message = "Transcript request marked as complete", data = request });
    }
}