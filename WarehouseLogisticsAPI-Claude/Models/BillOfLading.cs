using WarehouseLogistics_Claude.Models.Interfaces;

namespace WarehouseLogistics_Claude.Models
{
    public class BillOfLading : IBillOfLading
    {
        public string PartitionKey { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Possible values: Pending, Submitted, Created, In Transit, Delivered
        public string CustomerFirstName { get; set; } = string.Empty;
        public string CustomerLastName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public List<LineEntry> LineEntries { get; set; } = new List<LineEntry>();
    }
}
