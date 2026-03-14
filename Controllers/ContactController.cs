using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Backend.DTOs.Contact;
using SchoolSystem.Backend.Services;

namespace SchoolSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController(ContactMessageService contactMessageService) : ControllerBase
{
    // POST /api/contact — public
    [HttpPost]
    public async Task<IActionResult> CreateContactMessage([FromBody] CreateContactMessageDto dto)
    {
        var message = await contactMessageService.CreateAsync(dto);
        return CreatedAtAction(
            nameof(GetContactMessage),
            new { id = message.Id },
            new { id = message.Id, message = "Your message has been received." });
    }

    // GET /api/contact
    [Authorize(Roles = "SystemOwner, Manager")]
    [HttpGet]
    public async Task<IActionResult> GetContactMessages(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool unresolvedOnly = false)
    {
        var (items, total) = await contactMessageService.GetPagedAsync(pageNumber, pageSize, unresolvedOnly);
        return Ok(new
        {
            data = items,
            pagination = new { pageNumber, pageSize, totalRecords = total }
        });
    }

    // GET /api/contact/{id}
    [Authorize(Roles = "SystemOwner, Manager")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetContactMessage(Guid id)
    {
        var message = await contactMessageService.GetByIdAsync(id);
        return message == null ? NotFound() : Ok(message);
    }

    // PUT /api/contact/{id}/resolve
    [Authorize(Roles = "SystemOwner, Manager")]
    [HttpPut("{id:guid}/resolve")]
    public async Task<IActionResult> Resolve(Guid id)
    {
        var message = await contactMessageService.ResolveAsync(id);
        return Ok(new { message = "Contact message resolved", data = message });
    }
}
