using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Backend.Services.BaseService;

namespace SchoolSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseController<TEntity>(BaseService<TEntity> service) : ControllerBase
    where TEntity : class
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        return Ok(await service.GetByIdAsync(id));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await service.GetAllAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TEntity entity)
    {
        return Ok(await service.AddAsync(entity));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TEntity entity)
    {
        // assumes entity.Id = id
        return Ok(await service.UpdateAsync(entity));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return Ok(await service.DeleteAsync(id));
    }
}