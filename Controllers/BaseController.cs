using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Interfaces;

namespace SchoolSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseController<TEntity>(BaseService<TEntity> service) : ControllerBase
    where TEntity : class, IEntity, IHasTenant
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await service.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [Authorize(Roles = "SystemOwner, Manager, SchoolAdmin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await service.GetAllAsync());
    }

    [Authorize(Roles = "SystemOwner, Manager, SchoolAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TEntity entity)
    {
        var created = await service.AddAsync(entity);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [Authorize(Roles = "SystemOwner, Manager, SchoolAdmin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TEntity entity)
    {
        if (id != entity.Id)
            return BadRequest("ID mismatch");

        return Ok(await service.UpdateAsync(entity));
    }

    [Authorize(Roles = "SystemOwner, Manager, SchoolAdmin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return await service.DeleteAsync(id) ? NoContent() : NotFound();
    }

    [Authorize(Roles = "SystemOwner, Manager, SchoolAdmin")]
    [HttpPost("bulk")]
    public async Task<IActionResult> BulkCreate([FromBody] List<TEntity> entities)
    {
        var result = await service.BulkCreate(entities);
        return Ok(result);
    }

    [Authorize(Roles = "SystemOwner, Manager, SchoolAdmin")]
    [HttpPut("bulk")]
    public async Task<IActionResult> BulkUpdate([FromBody] List<TEntity> entities)
    {
        var result = await service.BulkUpdate(entities);
        return Ok(result);
    }

    [Authorize(Roles = "SystemOwner, Manager, SchoolAdmin")]
    [HttpDelete("bulk")]
    public async Task<IActionResult> BulkDelete([FromBody] List<TEntity> entities)
    {
        var result = await service.BulkDelete(entities);
        return Ok(result);
    }
}