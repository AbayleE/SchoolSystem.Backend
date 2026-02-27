using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Backend.DTOs.Users;
using SchoolSystem.Backend.Extensions;
using SchoolSystem.Backend.Services;
using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Enums;

namespace SchoolSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(UserService userService) : ControllerBase
{
    // GET /api/users/profile — current user's own profile
    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.GetUserId();
        var user = await userService.GetUserByIdAsync(userId);
        if (user == null) return NotFound();
        return Ok(new
        {
            user.Id,
            user.TenantId,
            user.Email,
            user.Phone,
            user.Role,
            Name = user.Name != null ? $"{user.Name.FirstName} {user.Name.LastName}" : "",
            user.CreatedAt
        });
    }

    // GET /api/users — all users in tenant
    [Authorize(Roles = "SystemOwner, SchoolAdmin, Manager")]
    [HttpGet]
    public async Task<IActionResult> GetUsers()
        => Ok(await userService.GetUsersAsync());

    // GET /api/users/role/{role}
    [Authorize(Roles = "SystemOwner, SchoolAdmin, Manager")]
    [HttpGet("role/{role}")]
    public async Task<IActionResult> GetUsersByRole(UserRole role)
        => Ok(await userService.GetUsersByRoleAsync(role));

    // GET /api/users/{id}
    [Authorize(Roles = "SystemOwner, SchoolAdmin, Manager")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await userService.GetUserByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    // PUT /api/users/{id} — update profile (self or admin)
    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
    {
        var currentUserId = User.GetUserId();
        var isAdmin = User.IsInRole("SystemOwner") || User.IsInRole("Manager") || User.IsInRole("SchoolAdmin");

        if (currentUserId != id && !isAdmin)
            return Forbid();

        var user = await userService.UpdateUserAsync(id, dto);
        return Ok(new { message = "User updated successfully", data = user });
    }

    // PATCH /api/users/password — update own password
    [Authorize]
    [HttpPatch("password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto dto)
    {
        var userId = User.GetUserId();
        await userService.UpdatePasswordAsync(userId, dto);
        return Ok(new { message = "Password updated successfully" });
    }

    // PATCH /api/users/{id}/status — admin activates/deactivates user
    [Authorize(Roles = "SystemOwner, SchoolAdmin")]
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetUserStatusDto dto)
    {
        await userService.SetActiveStatusAsync(id, dto.IsActive);
        return Ok(new { message = $"User {(dto.IsActive ? "activated" : "deactivated")} successfully" });
    }

    // DELETE /api/users/{id}
    [Authorize(Roles = "SystemOwner, SchoolAdmin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var success = await userService.DeleteUserAsync(id);
        return success ? NoContent() : NotFound();
    }
}
