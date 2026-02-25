using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Backend.DTOs.Contact;
using SchoolSystem.Backend.Extensions;
using SchoolSystem.Backend.Interface;
using SchoolSystem.Backend.Services;

namespace SchoolSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController(
    ContactMessageService contactMessageService,
    ITenantContext tenantContext,
    ILogger<ContactController> logger) : ControllerBase
{
    // ---------------------------------------------------------
    // POST /api/contact
    // Accept contact form data, validate, store to database
    // Send to admin email (optional)
    // No auth required (public endpoint)
    // ---------------------------------------------------------
    [HttpPost]
    public async Task<IActionResult> CreateContactMessage([FromBody] CreateContactMessageDto dto)
    {
        try
        {
            // Get tenant from header or context
            var tenantId = HttpContext.GetTenantIdFromHeader();
            if (tenantId == Guid.Empty && tenantContext.TenantId != Guid.Empty)
                tenantId = tenantContext.TenantId;

            if (tenantId == Guid.Empty)
                return BadRequest(new { message = "Invalid tenant context" });

            var message = await contactMessageService.CreateContactMessageAsync(dto, tenantId);

            return CreatedAtAction(
                nameof(GetContactMessage),
                new { id = message.Id },
                new { id = message.Id, message = "Your message has been received. We'll get back to you shortly." });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Invalid contact message data");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating contact message");
            return StatusCode(500, new { message = "An error occurred while processing your message" });
        }
    }

    // ---------------------------------------------------------
    // GET /api/contact/messages
    // List all contact messages
    // Admin/SchoolAdmin only
    // ---------------------------------------------------------
    [Authorize(Roles = "SystemOwner, SchoolAdmin, Manager")]
    [HttpGet("messages")]
    public async Task<IActionResult> GetContactMessages(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var messages = await contactMessageService.GetContactMessagesAsync(
                tenantContext.TenantId,
                pageNumber,
                pageSize);

            return Ok(new
            {
                data = messages,
                pagination = new
                {
                    pageNumber = pageNumber,
                    pageSize = pageSize,
                    totalRecords = messages.Count
                }
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching contact messages");
            return StatusCode(500, new { message = "An error occurred while fetching messages" });
        }
    }

    // ---------------------------------------------------------
    // GET /api/contact/:id
    // Get single contact message (optional)
    // ---------------------------------------------------------
    [Authorize(Roles = "SystemOwner, SchoolAdmin, Manager")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetContactMessage(Guid id)
    {
        try
        {
            var message = await contactMessageService.GetContactMessageByIdAsync(id, tenantContext.TenantId);

            if (message == null)
                return NotFound(new { message = "Contact message not found" });

            // Mark as read
            //if (!message.IsRead)
               // await contactMessageService.MarkAsReadAsync(id, tenantContext.TenantId);

            return Ok(message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching contact message");
            return StatusCode(500, new { message = "An error occurred while fetching the message" });
        }
    }

    // ---------------------------------------------------------
    // PUT /api/contact/:id/mark-read
    // Mark message as read
    // ---------------------------------------------------------
    [Authorize(Roles = "SystemOwner, SchoolAdmin, Manager")]
    [HttpPut("{id:guid}/mark-read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        try
        {
            var message = await contactMessageService.MarkAsReadAsync(id, tenantContext.TenantId);

            if (message == null)
                return NotFound(new { message = "Contact message not found" });

            return Ok(new { message = "Message marked as read", data = message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error marking message as read");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }
}
