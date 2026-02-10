using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Backend.DTOs.Invitations;
using SchoolSystem.Backend.Interface;

namespace SchoolSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvitationsController(IInvitationService invitationService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "SchoolAdmin,Manager,SystemOwner")]
    public async Task<IActionResult> PostInvitation(CreateInvitationDto dto)
    {
        // Extract sender info from JWT
        var senderId = User.FindFirst("UserId")?.Value;
        var senderRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (senderId == null || senderRole == null)
            return Unauthorized("Invalid token");

        var invitation = await invitationService.CreateInvitationAsync(dto);
        return Ok(new
        {
            invitation.Id,
            invitation.Token,
            invitation.Email,
            invitation.Role,
            invitation.ExpiresAt
        });
    }

    [HttpGet("token/{token}")]
    public async Task<IActionResult> GetInvitationByToken(string token)
    {
        var invitation = await invitationService.GetInvitationByTokenAsync(token);
        if (invitation == null)
            return NotFound("Invalid or expired invitation");
        return Ok(new
        {
            invitation.Email,
            Role = invitation.Role.ToString(),
            invitation.TenantId
        });
    }
}