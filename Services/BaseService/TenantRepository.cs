using Microsoft.EntityFrameworkCore;
using SchoolSystem.Backend.Interface;
using SchoolSystem.Domain.Interfaces;

namespace SchoolSystem.Backend.Services.BaseService;


// Extends BaseRepository with tenant isolation.
// Every read/write is automatically scoped to the current tenant from the JWT.
// Use this for all school-scoped entities: Student, Teacher, Class, Grade, etc.


public class TenantRepository<TEntity>(DbContext context, ITenantContext tenant)
    : BaseRepository<TEntity>(context)
    where TEntity : class, IEntity, IHasTenant
{
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();
    private readonly DbContext _context1 = context;

    private void EnsureTenant()
    {
        if (tenant.TenantId == Guid.Empty)
            throw new InvalidOperationException("Tenant context is required for this operation.");
    }

    public override Task<TEntity?> GetByIdAsync(Guid id)
    {
        EnsureTenant();
        return _dbSet
            .Where(x => x.Id == id && x.TenantId == tenant.TenantId && !x.IsDeleted)
            .FirstOrDefaultAsync();
    }

    public override Task<List<TEntity>> GetAllAsync()
    {
        EnsureTenant();
        return _dbSet
            .Where(x => x.TenantId == tenant.TenantId && !x.IsDeleted)
            .ToListAsync();
    }

    public override async Task<TEntity> AddAsync(TEntity entity)
    {
        EnsureTenant();
        entity.TenantId = tenant.TenantId; 
        return await base.AddAsync(entity);
    }

    public override async Task<TEntity> UpdateAsync(TEntity entity)
    {
        EnsureTenant();
        var existing = await GetByIdAsync(entity.Id);
        if (existing == null)
            throw new UnauthorizedAccessException("Entity not found or does not belong to this tenant.");
        entity.TenantId = tenant.TenantId;
        return await base.UpdateAsync(entity);
    }

    public override async Task<bool> DeleteAsync(Guid id)
    {
        EnsureTenant();
        var existing = await GetByIdAsync(id);
        if (existing == null) return false;
        return await base.DeleteAsync(id);
    }

    public override async Task<List<TEntity>> BulkAddAsync(List<TEntity> entities)
    {
        EnsureTenant();
        foreach (var e in entities)
            e.TenantId = tenant.TenantId;
        return await base.BulkAddAsync(entities);
    }
    
    public override async Task<List<TEntity>> BulkUpdateAsync(List<TEntity> entities)
    {
        EnsureTenant();
        var ids = entities.Select(x => x.Id).ToList();
        
        var existingEntities = await _dbSet
            .Where(x => ids.Contains(x.Id) && x.TenantId == tenant.TenantId && !x.IsDeleted)
            .ToListAsync();
        
        if (existingEntities.Count != ids.Count)
            throw new InvalidOperationException("One or more entities not found or belong to a different tenant.");
  
        return await base.BulkUpdateAsync(entities);
    }

    public override async Task<bool> BulkDeleteAsync(List<Guid> ids)
    {
        EnsureTenant();
        var entities = await _dbSet
            .Where(x => ids.Contains(x.Id) && x.TenantId == tenant.TenantId && !x.IsDeleted)
            .ToListAsync();

        foreach (var e in entities)
        {
            e.IsDeleted = true;
            e.DeletedAt = DateTime.UtcNow;
            e.UpdatedAt = DateTime.UtcNow;
        }
        await _context1.SaveChangesAsync();
        return true;
    }
}