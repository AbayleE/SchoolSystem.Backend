using Microsoft.EntityFrameworkCore;
using SchoolSystem.Domain.Interfaces;

namespace SchoolSystem.Backend.Services.BaseService;

// Handles CRUD for entities that do NOT require tenant scoping
// e.g. Application, AuditLog, ContactMessage, FileResource, SystemSettings


public class BaseRepository<TEntity>(DbContext context)
    where TEntity : class, IEntity
{
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();
    private readonly DbContext _context = context;
    
    public virtual IQueryable<TEntity> GetQueryable()
        => _dbSet.Where(x => !x.IsDeleted);
    
    public virtual Task<TEntity?> GetByIdAsync(Guid id)
    {
        return _dbSet
            .Where(x => x.Id == id && !x.IsDeleted)
            .FirstOrDefaultAsync();
    }

    public virtual Task<List<TEntity>> GetAllAsync()
    {
        return _dbSet
            .Where(x => !x.IsDeleted)
            .ToListAsync();
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) return false;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public virtual async Task<List<TEntity>> BulkAddAsync(List<TEntity> entities)
    {
        foreach (var e in entities)
        {
            e.Id = Guid.NewGuid();
            e.CreatedAt = DateTime.UtcNow;
            e.UpdatedAt = DateTime.UtcNow;
        }
        await _dbSet.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
        return entities;
    }

    public virtual async Task<List<TEntity>> BulkUpdateAsync(List<TEntity> entities)
    {
        foreach (var e in entities)
        {
            e.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(e);
        }
        await _context.SaveChangesAsync();
        return entities;
    }

    public virtual async Task<bool> BulkDeleteAsync(List<Guid> ids)
    {
        var entities = await _dbSet
            .Where(x => ids.Contains(x.Id) && !x.IsDeleted)
            .ToListAsync();

        foreach (var e in entities)
        {
            e.IsDeleted = true;
            e.DeletedAt = DateTime.UtcNow;
            e.UpdatedAt = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();
        return true;
    }
}
