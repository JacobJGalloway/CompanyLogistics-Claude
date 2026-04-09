using Microsoft.EntityFrameworkCore;
using WarehouseLogistics_Claude.Data.Interfaces;
using WarehouseLogistics_Claude.Models;

namespace WarehouseLogistics_Claude.Data.Repositories
{
    public class WarehouseRepository(LogisticsReadContext readContext) : IWarehouseRepository
    {
        public async Task<IEnumerable<Warehouse>> GetAllAsync()
        {
            return await readContext.Warehouses.AsNoTracking().ToListAsync();
        }
    }
}
