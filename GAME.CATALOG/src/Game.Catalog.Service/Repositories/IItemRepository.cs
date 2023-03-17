using Game.Catalog.Service.Entities;

namespace Game.Catalog.Service.Repositories
{
    public interface IItemRepository
    {
        Task CreateAsync(Item entity);
        Task<IReadOnlyCollection<Item>> GetAllItemsAsync();
        Task<Item> GetItemAsync(Guid id);
        Task RemoveAsync(Item entity);
        Task UpdateAsync(Item entity);
    }
}