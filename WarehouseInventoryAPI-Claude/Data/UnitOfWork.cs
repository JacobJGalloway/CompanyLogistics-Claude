using Microsoft.EntityFrameworkCore;
using WarehouseInventory_Claude.Data.Interfaces;
using WarehouseInventory_Claude.Data.Repositories;
using WarehouseInventory_Claude.Models;

namespace WarehouseInventory_Claude.Data
{
    public class UnitOfWork(InventoryContext context, InventoryReadContext readContext) : IUnitOfWork
    {
        private IClothingRepository? _clothing;
        private IPPERepository? _ppe;
        private IToolRepository? _tools;

        public IClothingRepository Clothing => _clothing ??= new ClothingRepository(context, readContext);
        public IPPERepository PPE => _ppe ??= new PPERepository(context, readContext);
        public IToolRepository Tools => _tools ??= new ToolRepository(context, readContext);

        public async Task<List<Clothing>> GetClothingBySKUIdAsync(string skuId) =>
            await readContext.Clothing.Where(c => c.SKUMarker == skuId).AsNoTracking().ToListAsync();

        public async Task<List<PPE>> GetPPEBySKUIdAsync(string skuId) =>
            await readContext.PPE.Where(p => p.SKUMarker == skuId).AsNoTracking().ToListAsync();

        public async Task<List<Tool>> GetToolBySKUIdAsync(string skuId) =>
            await readContext.Tool.Where(t => t.SKUMarker == skuId).AsNoTracking().ToListAsync();

        public async Task<int> SaveChangesAsync() => await context.SaveChangesAsync();

        public void Dispose()
        {
            context.Dispose();
            readContext.Dispose();
        }
    }
}
