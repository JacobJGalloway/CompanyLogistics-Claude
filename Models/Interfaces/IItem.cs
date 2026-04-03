namespace WarehouseInventory_Claude.Models.Interfaces
{
    public interface IItem
    {
        string PartitionKey { get; set; }
        string RowKey { get; set; }
        string SKUMarker { get; set; }

        DateTime UnloadedDate { get; set; }
    }
}