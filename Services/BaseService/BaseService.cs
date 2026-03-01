using SchoolSystem.Domain.Interfaces;

namespace SchoolSystem.Backend.Services.BaseService;


// Application-layer service wrapping BaseRepository.
// Use for non-tenant entities: Application, AuditLog, ContactMessage, etc.

public class BaseService<TEntity>(BaseRepository<TEntity> repo)
    where TEntity : class, IEntity
{
    public Task<TEntity?> GetByIdAsync(Guid id) => repo.GetByIdAsync(id);
    public Task<List<TEntity>> GetAllAsync() => repo.GetAllAsync();
    public Task<TEntity> AddAsync(TEntity entity) => repo.AddAsync(entity);
    public Task<TEntity> UpdateAsync(TEntity entity) => repo.UpdateAsync(entity);
    public Task<bool> DeleteAsync(Guid id) => repo.DeleteAsync(id);
    public Task<List<TEntity>> BulkAddAsync(List<TEntity> entities) => repo.BulkAddAsync(entities);
    public Task<List<TEntity>> BulkUpdateAsync(List<TEntity> entities) => repo.BulkUpdateAsync(entities);
    public Task<bool> BulkDeleteAsync(List<Guid> ids) => repo.BulkDeleteAsync(ids);
}

