using SchoolSystem.Domain.Interfaces;

namespace SchoolSystem.Backend.Services.BaseService;

public class BaseService<TEntity>(BaseRepository<TEntity> repo)
    where TEntity : class, IEntity, IHasTenant
{
    private readonly BaseRepository<TEntity> _repo = repo;

    public Task<TEntity?> GetByIdAsync(Guid id)
    {
        return _repo.GetByIdAsync(id);
    }

    public Task<List<TEntity>> GetAllAsync()
    {
        return _repo.GetAllAsync();
    }

    public Task<TEntity> AddAsync(TEntity entity)
    {
        return _repo.AddAsync(entity);
    }

    public Task<TEntity> UpdateAsync(TEntity entity)
    {
        return _repo.UpdateAsync(entity);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return _repo.DeleteAsync(id);
    }
}