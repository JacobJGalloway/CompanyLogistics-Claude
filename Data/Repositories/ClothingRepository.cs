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

        public async Task<Clothing?> GetBySKUIdAsync(string skuId)
        {
            return await _context.FindClothingBySKUAsync(skuId);
        }

        public async Task<Clothing> AddAsync(Clothing item)
        {
            _context.Clothing.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task UpdateBySKUIdAsync(string skuId, Clothing item)
        {
            var existingItem = await GetBySKUIdAsync(skuId);
            if (existingItem is null) return;

            _context.Entry(existingItem).CurrentValues.SetValues(item);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteBySKUIdAsync(string skuId)
        {
            List<Clothing> items = await _context.Clothing.Where(c => c.SKUMarker == skuId).ToListAsync();
            if (items.Count == 0) return false;
            _context.Clothing.RemoveRange(items);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

