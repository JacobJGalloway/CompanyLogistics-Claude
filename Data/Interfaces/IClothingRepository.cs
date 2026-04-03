using WarehouseInventory_Claude.Models;

namespace WarehouseInventory_Claude.Data.Interfaces
{
    public interface IClothingRepository
    {
        Task<IEnumerable<Clothing>> GetAllAsync();
        Task<Clothing?> GetBySKUIdAsync(string skuId);
        Task<Clothing> AddAsync(Clothing item);
        Task UpdateBySKUIdAsync(string skuId, Clothing item);
        Task<bool> DeleteBySKUIdAsync(string skuId);
    }
}

