using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WarehouseInventory_Claude.Data;
using WarehouseInventory_Claude.Data.Repositories;
using WarehouseInventory_Claude.Models;

namespace WarehouseInventory_Claude.Tests.Repositories
{
    public class ClothingRepositoryTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly InventoryContext _context;
        private readonly ClothingRepository _repository;

        public ClothingRepositoryTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            var options = new DbContextOptionsBuilder<InventoryContext>()
                .UseSqlite(_connection)
                .Options;
            _context = new InventoryContext(options);
            _context.Database.EnsureCreated();
            _repository = new ClothingRepository(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllItems()
        {
            _context.Clothing.AddRange(
                new Clothing { PartitionKey = "pk1", SKUMarker = "CLTH001", UnloadedDate = DateTime.UtcNow },
                new Clothing { PartitionKey = "pk2", SKUMarker = "CLTH002", UnloadedDate = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetBySKUIdAsync_ReturnsMatchingItems_WhenFound()
        {
            _context.Clothing.AddRange(
                new Clothing { PartitionKey = "pk1", SKUMarker = "CLTH001", UnloadedDate = DateTime.UtcNow },
                new Clothing { PartitionKey = "pk2", SKUMarker = "CLTH001", UnloadedDate = DateTime.UtcNow },
                new Clothing { PartitionKey = "pk3", SKUMarker = "CLTH002", UnloadedDate = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            var result = await _repository.GetBySKUIdAsync("CLTH001");

            Assert.Equal(2, result.Count);
            Assert.All(result, c => Assert.Equal("CLTH001", c.SKUMarker));
        }

        [Fact]
        public async Task GetBySKUIdAsync_ReturnsEmptyList_WhenNotFound()
        {
            var result = await _repository.GetBySKUIdAsync("CLTH999");

            Assert.Empty(result);
        }

        [Fact]
        public async Task AddAsync_StagesItem_PersistedAfterSave()
        {
            var item = new Clothing { PartitionKey = "pk1", SKUMarker = "CLTH001", UnloadedDate = DateTime.UtcNow };

            await _repository.AddAsync(item);
            await _context.SaveChangesAsync();

            Assert.Equal(1, await _context.Clothing.CountAsync());
        }

        [Fact]
        public async Task UpdateBySKUIdAsync_UpdatesByPartitionKey_WhenMatch()
        {
            var original = new Clothing { PartitionKey = "pk1", SKUMarker = "CLTH001", UnloadedDate = DateTime.UtcNow };
            var other = new Clothing { PartitionKey = "pk2", SKUMarker = "CLTH001", UnloadedDate = DateTime.UtcNow };
            _context.Clothing.AddRange(original, other);
            await _context.SaveChangesAsync();

            var updated = new Clothing { PartitionKey = "pk1", SKUMarker = "CLTH001", UnloadedDate = DateTime.UtcNow.AddDays(1) };
            await _repository.UpdateBySKUIdAsync("CLTH001", updated);
            await _context.SaveChangesAsync();

            var result = await _context.Clothing.FindAsync("pk1");
            Assert.Equal(updated.UnloadedDate, result!.UnloadedDate);
        }

        [Fact]
        public async Task UpdateBySKUIdAsync_FallsBackToFirst_WhenNoPartitionKeyMatch()
        {
            var item = new Clothing { PartitionKey = "pk1", SKUMarker = "CLTH001", UnloadedDate = DateTime.UtcNow };
            _context.Clothing.Add(item);
            await _context.SaveChangesAsync();

            var updated = new Clothing { PartitionKey = "pk-nomatch", SKUMarker = "CLTH001", UnloadedDate = DateTime.UtcNow.AddDays(1) };
            await _repository.UpdateBySKUIdAsync("CLTH001", updated);
            await _context.SaveChangesAsync();

            var result = await _context.Clothing.FindAsync("pk1");
            Assert.Equal(updated.UnloadedDate, result!.UnloadedDate);
        }

        [Fact]
        public async Task UpdateBySKUIdAsync_DoesNothing_WhenSkuNotFound()
        {
            var item = new Clothing { PartitionKey = "pk1", SKUMarker = "CLTH001", UnloadedDate = DateTime.UtcNow };
            _context.Clothing.Add(item);
            await _context.SaveChangesAsync();

            await _repository.UpdateBySKUIdAsync("CLTH999", new Clothing { PartitionKey = "pk1", SKUMarker = "CLTH999", UnloadedDate = DateTime.UtcNow.AddDays(1) });
            await _context.SaveChangesAsync();

            var result = await _context.Clothing.FindAsync("pk1");
            Assert.Equal(item.UnloadedDate, result!.UnloadedDate);
        }

        [Fact]
        public async Task DeleteBySKUIdAsync_ReturnsFalse_WhenNotFound()
        {
            var result = await _repository.DeleteBySKUIdAsync("CLTH999");

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteBySKUIdAsync_ReturnsTrue_AndRemovesAllMatchingItems()
        {
            _context.Clothing.AddRange(
                new Clothing { PartitionKey = "pk1", SKUMarker = "CLTH001", UnloadedDate = DateTime.UtcNow },
                new Clothing { PartitionKey = "pk2", SKUMarker = "CLTH001", UnloadedDate = DateTime.UtcNow },
                new Clothing { PartitionKey = "pk3", SKUMarker = "CLTH002", UnloadedDate = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            var result = await _repository.DeleteBySKUIdAsync("CLTH001");
            await _context.SaveChangesAsync();

            Assert.True(result);
            Assert.Equal(1, await _context.Clothing.CountAsync());
        }
    }
}
