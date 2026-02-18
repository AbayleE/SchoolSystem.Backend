using Microsoft.EntityFrameworkCore;
using SchoolSystem.Backend.Interface;
using SchoolSystem.Domain.Interfaces;

namespace SchoolSystem.Backend.Services.BaseService;

public class BaseRepository<TEntity>(DbContext context, ITenantContext tenant)
    where TEntity : class, IEntity, IHasTenant
{
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    public Task<TEntity?> GetByIdAsync(Guid id)
    {
        return _dbSet
            .Where(x => x.Id == id && x.TenantId == tenant.TenantId)
            .FirstOrDefaultAsync();
    }

    public Task<List<TEntity>> GetAllAsync()
    {
        return _dbSet
            .Where(x => x.TenantId == tenant.TenantId)
            .ToListAsync();
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        entity.TenantId = tenant.TenantId;
        await _dbSet.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        entity.TenantId = tenant.TenantId;
        _dbSet.Update(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) return false;

        _dbSet.Remove(entity);
        await context.SaveChangesAsync();
        return true;
    }
}