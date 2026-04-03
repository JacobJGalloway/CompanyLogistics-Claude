using WarehouseLogistics_Claude.Models.Interfaces;

namespace WarehouseLogistics_Claude.Models
{
    public class LineEntry : ILineEntry
    {
        public string WarehouseId { get; set; } = string.Empty;
        public string SKUMarker { get; set; } = string.Empty;
        // Positive quantity = incoming; negative quantity = outgoing
        public int Quantity { get; set; }
    }
}
