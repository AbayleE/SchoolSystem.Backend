using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Backend.DTOs.Invitations;
using SchoolSystem.Backend.Extensions;
using SchoolSystem.Backend.Interface;
using SchoolSystem.Backend.Services;

namespace SchoolSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvitationsController(InvitationService invitationService, ITenantContext tenant) : ControllerBase
{
    // POST /api/invitations
    [Authorize(Roles = "SystemOwner, SchoolAdmin, Manager")]
    [HttpPost]
    public async Task<IActionResult> SendInvitation([FromBody] CreateInvitationDto dto)
    {
        var senderId = User.GetUserId();
        var invitation = await invitationService.SendInvitationAsync(dto, tenant.TenantId, senderId);
        return Ok(new
        {
            invitation.Id,
            invitation.Email,
            invitation.Role,
            invitation.ExpiresAt
        });
    }

    // GET /api/invitations — all invitations for this tenant
    [Authorize(Roles = "SystemOwner, SchoolAdmin, Manager")]
    [HttpGet]
    public async Task<IActionResult> GetInvitations()
        => Ok(await invitationService.GetAllInvitationsAsync());

    // DELETE /api/invitations/{id} — revoke
    [Authorize(Roles = "SystemOwner, SchoolAdmin, Manager")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RevokeInvitation(Guid id)
    {
        await invitationService.RevokeInvitationAsync(id);
        return NoContent();
    }
}