using System.Linq;
using Microsoft.EntityFrameworkCore;
using WarehouseInventory_Claude.Models;

namespace WarehouseInventory_Claude.Data

{
    public class InventoryContext(DbContextOptions<InventoryContext> options) : DbContext(options)
    {
        public DbSet<Clothing> Clothing => Set<Clothing>();
        public DbSet<PPE> PPE => Set<PPE>();
        public DbSet<Tool> Tools => Set<Tool>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Clothing>().HasKey(c => c.PartitionKey);
            modelBuilder.Entity<PPE>().HasKey(p => p.PartitionKey);
            modelBuilder.Entity<Tool>().HasKey(t => t.PartitionKey);
        }

        public async Task<List<Clothing>> GetClothingBySKUIdsync(string skuId)
        {
            var response = await Clothing.Where(c => c.SKUMarker == skuId).ToListAsync();

            if (response.Count == 0) return new List<Clothing>();

            return response;
        }

        public async Task<List<PPE>> GetPPEBySKUIdsync(string skuId)
        {
            var response = await PPE.Where(p => p.SKUMarker == skuId).ToListAsync();

            if (response.Count == 0) return new List<PPE>();

            return response;
        }

        public async Task<List<Tool>> GetToolBySKUIdsync(string skuId)
        {
            var response = await Tools.Where(t => t.SKUMarker == skuId).ToListAsync();

            if (response.Count == 0) return new List<Tool>();

            return response;
        }
    }
}