using Microsoft.EntityFrameworkCore;
using SchoolSystem.Backend.Interface;
using SchoolSystem.Domain.Interfaces;

namespace SchoolSystem.Backend.Services.BaseService;

public class BaseRepository<TEntity>(DbContext context, ITenantContext tenant)
    where TEntity : class, IEntity, IHasTenant
{
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    private void EnsureTenant()
    {
        if (tenant.TenantId == Guid.Empty)
            throw new InvalidOperationException("Tenant context is required for this operation.");
    }

    public Task<TEntity?> GetByIdAsync(Guid id)
    {
        EnsureTenant();
        return _dbSet
            .Where(x => x.Id == id && x.TenantId == tenant.TenantId)
            .FirstOrDefaultAsync();
    }

    public Task<List<TEntity>> GetAllAsync()
    {
        EnsureTenant();
        return _dbSet
            .Where(x => x.TenantId == tenant.TenantId)
            .ToListAsync();
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        EnsureTenant();
        entity.TenantId = tenant.TenantId;
        await _dbSet.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        EnsureTenant();
        entity.TenantId = tenant.TenantId;
        _dbSet.Update(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        EnsureTenant();
        var entity = await GetByIdAsync(id);
        if (entity == null) return false;

        _dbSet.Remove(entity);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<List<TEntity>> BulkDelete(List<TEntity> entities)
    {
        EnsureTenant();
        _dbSet.RemoveRange(entities);
        await context.SaveChangesAsync();
        return entities;
    }

    public async Task<List<TEntity>> BulkCreate(List<TEntity> entities)
    {
        EnsureTenant();
        foreach (var e in entities) e.TenantId = tenant.TenantId;
        await _dbSet.AddRangeAsync(entities);
        await context.SaveChangesAsync();
        return entities;
    }

    public async Task<List<TEntity>> BulkUpdate(List<TEntity> entities)
    {
        EnsureTenant();
        foreach (var entity in entities)
        {
            entity.TenantId = tenant.TenantId;
            _dbSet.Update(entity);
        }
        await context.SaveChangesAsync();
        return entities;
    }
}