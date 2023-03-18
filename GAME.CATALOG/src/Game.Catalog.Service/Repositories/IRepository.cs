using Game.Catalog.Service.Entities;

namespace Game.Catalog.Service.Repositories
{
    public interface IRepository<T> where T : IEntity
    {
        Task CreateAsync(T entity);
        Task<IReadOnlyCollection<T>> GetAllAsync();
        Task<T> GetAsync(Guid id);
        Task RemoveAsync(T entity);
        Task UpdateAsync(T entity);
    }
}