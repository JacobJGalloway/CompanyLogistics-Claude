using WarehouseInventory_Claude.Models;

namespace WarehouseInventory_Claude.Data.Interfaces
{
    public interface IToolRepository
    {
        Task<IEnumerable<Tool>> GetAllAsync();
        Task<Tool?> GetBySKUIdAsync(string skuId);
        Task<Tool> AddAsync(Tool item);
        Task UpdateBySKUIdAsync(string skuId, Tool item);
        Task<bool> DeleteBySKUIdAsync(string skuId);
    }
}

