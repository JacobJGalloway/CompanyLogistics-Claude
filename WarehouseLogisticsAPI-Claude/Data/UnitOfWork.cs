using WarehouseLogistics_Claude.Data.Interfaces;
using WarehouseLogistics_Claude.Data.Repositories;

namespace WarehouseLogistics_Claude.Data
{
    public class UnitOfWork(LogisticsContext context, LogisticsReadContext readContext) : IUnitOfWork
    {
        private IBillOfLadingRepository? _billsOfLading;
        private ILineEntryRepository? _lineEntries;
        private IWarehouseRepository? _warehouses;
        private IStoreRepository? _stores;

        public IBillOfLadingRepository BillsOfLading => _billsOfLading ??= new BillOfLadingRepository(context, readContext);
        public ILineEntryRepository LineEntries => _lineEntries ??= new LineEntryRepository(context, readContext);
        public IWarehouseRepository Warehouses => _warehouses ??= new WarehouseRepository(readContext);
        public IStoreRepository Stores => _stores ??= new StoreRepository(readContext);

        public async Task<int> SaveChangesAsync() => await context.SaveChangesAsync();

        public void Dispose()
        {
            context.Dispose();
            readContext.Dispose();
        }
    }
}
