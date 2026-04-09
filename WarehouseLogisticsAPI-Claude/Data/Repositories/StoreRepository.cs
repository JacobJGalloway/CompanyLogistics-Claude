using Microsoft.EntityFrameworkCore;
using WarehouseLogistics_Claude.Data.Interfaces;
using WarehouseLogistics_Claude.Models;

namespace WarehouseLogistics_Claude.Data.Repositories
{
    public class StoreRepository(LogisticsReadContext readContext) : IStoreRepository
    {
        public async Task<IEnumerable<Store>> GetAllAsync()
        {
            return await readContext.Stores.AsNoTracking().ToListAsync();
        }
    }
}
