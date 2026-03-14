using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Interfaces;

namespace SchoolSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseController<TEntity>(TenantService<TEntity> service) : ODataController
    where TEntity : class, IEntity, IHasTenant
{
    
    [HttpGet("query")]
    [Authorize]
    [EnableQuery(PageSize = 100, MaxExpansionDepth = 3)]
    public IQueryable<TEntity> Query() => service.GetQueryable();
  
    [Authorize]
    [HttpGet("{id:guid}")]
    
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await service.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await service.GetAllAsync());
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] TEntity entity)
    {
        var created = await service.AddAsync(entity);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }
    
    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] TEntity entity)
    {
        if (id != entity.Id)
            return BadRequest(new { message = "ID mismatch" });
        
        return Ok(await service.UpdateAsync(entity));
    }
    
    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        return await service.DeleteAsync(id) ? NoContent() : NotFound();
    }
    
    [HttpPost("bulk")]
    [Authorize]
    public async Task<IActionResult> BulkCreate([FromBody] List<TEntity> entities)
        => Ok(await service.BulkAddAsync(entities));

    [HttpPut("bulk")]
    [Authorize]
    public async Task<IActionResult> BulkUpdate([FromBody] List<TEntity> entities)
        => Ok(await service.BulkUpdateAsync(entities));

    [HttpDelete("bulk")]
    [Authorize]
    public async Task<IActionResult> BulkDelete([FromBody] List<Guid> ids)  
        => await service.BulkDeleteAsync(ids) ? NoContent() : NotFound();
}