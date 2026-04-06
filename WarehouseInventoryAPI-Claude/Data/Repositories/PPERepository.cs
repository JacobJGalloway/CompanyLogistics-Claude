using Microsoft.EntityFrameworkCore;
using WarehouseInventory_Claude.Data.Interfaces;
using WarehouseInventory_Claude.Models;

namespace WarehouseInventory_Claude.Data.Repositories
{
    public class PPERepository(InventoryContext context) : IPPERepository
    {
        private readonly InventoryContext _context = context;

        public async Task<IEnumerable<PPE>> GetAllAsync()
        {
            return await _context.PPE.ToListAsync();
        }

        public async Task<List<PPE>> GetBySKUIdAsync(string skuId)
        {
            return await _context.GetPPEBySKUIdsync(skuId);
        }

        public async Task<PPE> AddAsync(PPE item)
        {
            _context.PPE.Add(item);
            return item;
        }

        public async Task UpdateBySKUIdAsync(string skuId, PPE item)
        {
            var existingItems = await GetBySKUIdAsync(skuId);
            if (existingItems.Count == 0) return;

            var target = existingItems.FirstOrDefault(c => c.PartitionKey == item.PartitionKey)
                         ?? existingItems[0];

            _context.Entry(target).CurrentValues.SetValues(item);
        }

        public async Task<bool> DeleteBySKUIdAsync(string skuId)
        {
            List<PPE> items = await _context.PPE.Where(p => p.PartitionKey == skuId).ToListAsync();
            if (items.Count == 0) return false;
            _context.PPE.RemoveRange(items);
            return true;
        }
    }
}