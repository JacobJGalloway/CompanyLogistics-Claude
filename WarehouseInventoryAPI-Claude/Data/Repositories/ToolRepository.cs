using Microsoft.EntityFrameworkCore;
using WarehouseInventory_Claude.Data.Interfaces;
using WarehouseInventory_Claude.Models;

namespace WarehouseInventory_Claude.Data.Repositories
{
    public class ToolRepository(InventoryContext context) : IToolRepository
    {
        private readonly InventoryContext _context = context;

        public async Task<IEnumerable<Tool>> GetAllAsync()
        {
            return await _context.Tools.ToListAsync();
        }

        public async Task<List<Tool>> GetBySKUIdAsync(string skuId)
        {
            return await _context.GetToolBySKUIdsync(skuId);
        }

        public async Task<Tool> AddAsync(Tool item)
        {
            _context.Tools.Add(item);
            return item;
        }

        public async Task UpdateBySKUIdAsync(string skuId, Tool item)
        {
            var existingItems = await GetBySKUIdAsync(skuId);
            if (existingItems.Count == 0) return;

            var target = existingItems.FirstOrDefault(t => t.PartitionKey == item.PartitionKey)
                         ?? existingItems[0];
                         
            _context.Entry(target).CurrentValues.SetValues(item);
        }

        public async Task<bool> DeleteBySKUIdAsync(string skuId)
        {
            List<Tool> items = await _context.Tools.Where(t => t.PartitionKey == skuId).ToListAsync();
            if (items.Count == 0) return false;
            _context.Tools.RemoveRange(items);
            return true;
        }
    }
}