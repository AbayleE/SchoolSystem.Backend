namespace SchoolSystem.Backend.Services.BaseService;

public abstract class BaseService<TEntity>(BaseRepository<TEntity> repo)
    where TEntity : class
{
    public Task<TEntity?> GetByIdAsync(Guid id)
    {
        return repo.GetByIdAsync(id);
    }

    public Task<List<TEntity>> GetAllAsync()
    {
        return repo.GetAllAsync();
    }

    public Task<TEntity> AddAsync(TEntity entity)
    {
        return repo.AddAsync(entity);
    }

    public Task<TEntity> UpdateAsync(TEntity entity)
    {
        return repo.UpdateAsync(entity);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return repo.DeleteAsync(id);
    }
}