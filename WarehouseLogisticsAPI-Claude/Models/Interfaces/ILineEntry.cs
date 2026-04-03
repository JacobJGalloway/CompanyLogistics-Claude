namespace WarehouseLogistics_Claude.Models.Interfaces
{
    public interface ILineEntry
    {
        string WarehouseId { get; set; }
        string SKUMarker { get; set; }
        int Quantity { get; set; }
    }
}
