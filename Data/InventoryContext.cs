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

        public async Task<Clothing?> FindClothingBySKUAsync(string skuId)
        {
            var response = await Clothing.FirstOrDefaultAsync(c => c.SKUMarker == skuId);

            if (response is null || response == default(Clothing))
            {
                throw new KeyNotFoundException($"No clothing item found with SKU: {skuId}");
            }

            return response;
        }
    }
}
