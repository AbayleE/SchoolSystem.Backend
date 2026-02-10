using Microsoft.EntityFrameworkCore;
using SchoolSystem.Backend.Data;

namespace SchoolSystem.Backend.Services.BaseService;

public abstract class BaseRepository<TEntity>(SchoolDbContext context)
    where TEntity : class
{
    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await context.Set<TEntity>().FindAsync(id);
    }

    public async Task<List<TEntity>> GetAllAsync()
    {
        return await context.Set<TEntity>().ToListAsync();
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        context.Set<TEntity>().Add(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        context.Set<TEntity>().Update(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) return false;

        context.Set<TEntity>().Remove(entity);
        await context.SaveChangesAsync();
        return true;
    }
}