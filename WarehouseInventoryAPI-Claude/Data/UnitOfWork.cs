using WarehouseInventory_Claude.Data.Interfaces;
using WarehouseInventory_Claude.Data.Repositories;
using WarehouseInventory_Claude.Models;

namespace WarehouseInventory_Claude.Data
{
    public class UnitOfWork(InventoryContext context) : IUnitOfWork
    {
        private readonly InventoryContext _context = context;

        private IClothingRepository? _clothing;
        private IPPERepository? _ppe;
        private IToolRepository? _tools;

        public IClothingRepository Clothing => _clothing ??= new ClothingRepository(_context);
        public IPPERepository PPE => _ppe ??= new PPERepository(_context);
        public IToolRepository Tools => _tools ??= new ToolRepository(_context);

        public async Task<List<Clothing>> GetClothingBySKUIdAsync(string skuId) =>
            await _context.GetClothingBySKUIdsync(skuId);

        public async Task<List<PPE>> GetPPEBySKUIdAsync(string skuId) =>
            await _context.GetPPEBySKUIdsync(skuId);

        public async Task<List<Tool>> GetToolBySKUIdAsync(string skuId) =>
            await _context.GetToolBySKUIdsync(skuId);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
