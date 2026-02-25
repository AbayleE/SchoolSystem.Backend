using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Backend.DTOs.Users;
using SchoolSystem.Backend.Extensions;
using SchoolSystem.Backend.Services;
using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(UserService userService) : ControllerBase
{
    // ---------------------------------------------------------
    // GET /api/users/profile
    // Return current authenticated user info
    // ---------------------------------------------------------
    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized(new { message = "Invalid user context" });

        var user = await userService.GetUserByIdAsync(userId);
        if (user == null)
            return NotFound(new { message = "User not found" });

        return Ok(new
        {
            id = user.Id,
            tenantId = user.TenantId,
            email = user.Email,
            phone = user.Phone,
            role = user.Role.ToString(),
            name = user.Name != null ? $"{user.Name.FirstName} {user.Name.LastName}" : "",
            createdAt = user.CreatedAt
        });
    }

    // ---------------------------------------------------------
    // GET /api/users
    // List all users in tenant with pagination
    // Admin only
    // ---------------------------------------------------------
    [Authorize(Roles = "SystemOwner, Manager, SchoolAdmin")]
    [HttpGet]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? role = null)
    {
        // This should be extended from UserService to support pagination and filtering
        // For now, returning a basic implementation
        return Ok(new { message = "Use specific user endpoints" });
    }

    // ---------------------------------------------------------
    // GET /api/users/:id
    // Get user by ID
    // ---------------------------------------------------------
    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await userService.GetUserByIdAsync(id);
        return user is null ? NotFound() : Ok(user);
    }

    // ---------------------------------------------------------
    // GET /api/users/tenant/:tenantId
    // Get all users by tenant
    // Admin only
    // ---------------------------------------------------------
    [Authorize(Roles = "SystemOwner, Manager, SchoolAdmin")]
    [HttpGet("tenant/{tenantId:guid}")]
    public async Task<IActionResult> GetUsersByTenant(Guid tenantId)
    {
        var users = await userService.GetUsersByTenantAsync(tenantId);
        return Ok(users);
    }

    // ---------------------------------------------------------
    // POST /api/users
    // Create new user
    // Admin only
    // ---------------------------------------------------------
    [Authorize(Roles = "SystemOwner, Manager, SchoolAdmin")]
    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserDto dto)
    {
        var user = await userService.CreateUserAsync(dto);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    // ---------------------------------------------------------
    // PUT /api/users/:id
    // Update user info
    // Admin or self only
    // ---------------------------------------------------------
    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
    {
        var currentUserId = User.GetUserId();
        var isAdmin = User.IsInRole("SystemOwner") || User.IsInRole("Manager") || User.IsInRole("SchoolAdmin");

        // Only allow users to update their own profile or admins to update any profile
        if (currentUserId != id && !isAdmin)
            return Forbid("You can only update your own profile");

        var user = await userService.UpdateUserAsync(id, dto);
        if (user == null)
            return NotFound(new { message = "User not found" });

        return Ok(new { message = "User updated successfully", data = user });
    }

    // ---------------------------------------------------------
    // DELETE /api/users/:id
    // Delete user
    // Admin only
    // ---------------------------------------------------------
    [Authorize(Roles = "SystemOwner, SchoolAdmin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var success = await userService.DeleteUserAsync(id);
        return success ? NoContent() : NotFound(new { message = "User not found" });
    }

    [HttpPatch]
    public async Task<IActionResult> UpdatePassword ([FromBody] ForgotPasswordRequestDto forgotPasswordRequestDto)
    {
        var success = await userService.UpdatePasswordAsync(forgotPasswordRequestDto.Email, forgotPasswordRequestDto.NewPassword);

        return Ok(new { message = "User Password updated successfully" });
        
    }
}
