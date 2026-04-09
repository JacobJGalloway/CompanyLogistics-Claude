namespace WarehouseInventory_Claude.Data.Sync
{
    public record SyncJob(IReadOnlySet<Type> ChangedTypes);
}
