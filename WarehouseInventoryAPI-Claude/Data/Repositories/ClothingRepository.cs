using Microsoft.EntityFrameworkCore;
using WarehouseInventory_Claude.Data.Interfaces;
using WarehouseInventory_Claude.Models;

namespace WarehouseInventory_Claude.Data.Repositories
{
    public class ClothingRepository(InventoryContext context) : IClothingRepository
    {
        private readonly InventoryContext _context = context;

        public async Task<IEnumerable<Clothing>> GetAllAsync()
        {
            return await _context.Clothing.ToListAsync();
        }

        public async Task<List<Clothing>> GetBySKUIdAsync(string skuId)
        {
            var items = await _context.GetClothingBySKUIdsync(skuId);
            
            if (items.Count == 0) return new List<Clothing>();

            return items;
        }

        public async Task<Clothing> AddAsync(Clothing item)
        {
            _context.Clothing.Add(item);
            return item;
        }

        public async Task UpdateBySKUIdAsync(string skuId, Clothing item)
        {
            var existingItems = await GetBySKUIdAsync(skuId);
            if (existingItems.Count == 0) return;

            var target = existingItems.FirstOrDefault(c => c.PartitionKey == item.PartitionKey)
                         ?? existingItems[0];
            _context.Entry(target).CurrentValues.SetValues(item);
        }

        public async Task<bool> DeleteBySKUIdAsync(string skuId)
        {
            List<Clothing> items = await _context.Clothing.Where(c => c.SKUMarker == skuId).ToListAsync();
            if (items.Count == 0) return false;
            _context.Clothing.RemoveRange(items);
            return true;
        }
    }
}