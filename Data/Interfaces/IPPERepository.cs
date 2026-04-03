using WarehouseInventory_Claude.Models;

namespace WarehouseInventory_Claude.Data.Interfaces
{
    public interface IPPERepository
    {
        Task<IEnumerable<PPE>> GetAllAsync();
        Task<PPE?> GetBySKUIdAsync(string skuId);
        Task<PPE> AddAsync(PPE item);
        Task UpdateBySKUIdAsync(string skuId, PPE item);
        Task<bool> DeleteBySKUIdAsync(string skuId);
    }
}

