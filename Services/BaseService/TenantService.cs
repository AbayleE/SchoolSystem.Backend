using SchoolSystem.Domain.Interfaces;

namespace SchoolSystem.Backend.Services.BaseService;

// Application-layer service wrapping TenantRepository.
// Use for all school-scoped entities: Student, Teacher, Class, Grade, etc.
// All operations are automatically tenant-isolated.

public class TenantService<TEntity>(TenantRepository<TEntity> repo)
    where TEntity : class, IEntity, IHasTenant
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