using WarehouseLogistics_Claude.Models;

namespace WarehouseLogistics_Claude.Services.Interfaces
{
    public interface IBillOfLadingService
    {
        Task<string> CreateAsync(BillOfLading billOfLading);
        Task ProcessLocationStop(string transactionId, string locationId);
        Task<IEnumerable<BillOfLading>> GetAllAsync();
        Task<BillOfLading?> GetByTransactionIdAsync(string transactionId);
        Task<List<LineEntry>> GetLineEntriesByTransactionIdAsync(string transactionId);
    }
}
