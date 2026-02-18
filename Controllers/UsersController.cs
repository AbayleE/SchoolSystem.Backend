using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Backend.DTOs.Users;
using SchoolSystem.Backend.Services;
using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(UserService userService) : ControllerBase
{
    [Authorize(Roles = "SystemOwner, Manager, SchoolAdmin")]
    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserDto dto)
    {
        var user = await userService.CreateUserAsync(dto);
        return Ok(user);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await userService.GetUserByIdAsync(id);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpGet("tenant/{tenantId}")]
    public async Task<IActionResult> GetUsersByTenant(Guid tenantId)
    {
        var users = await userService.GetUsersByTenantAsync(tenantId);
        return Ok(users);
    }
}