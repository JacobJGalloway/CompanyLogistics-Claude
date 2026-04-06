using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WarehouseInventory_Claude.Data;
using WarehouseInventory_Claude.Data.Repositories;
using WarehouseInventory_Claude.Models;

namespace WarehouseInventory_Claude.Tests.Repositories
{
    public class ToolRepositoryTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly InventoryContext _context;
        private readonly ToolRepository _repository;

        public ToolRepositoryTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            var options = new DbContextOptionsBuilder<InventoryContext>()
                .UseSqlite(_connection)
                .Options;
            _context = new InventoryContext(options);
            _context.Database.EnsureCreated();
            _repository = new ToolRepository(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllItems()
        {
            _context.Tools.AddRange(
                new Tool { PartitionKey = "pk1", SKUMarker = "PWTL001", UnloadedDate = DateTime.UtcNow },
                new Tool { PartitionKey = "pk2", SKUMarker = "PWTL002", UnloadedDate = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetBySKUIdAsync_ReturnsMatchingItems_WhenFound()
        {
            _context.Tools.AddRange(
                new Tool { PartitionKey = "pk1", SKUMarker = "PWTL001", UnloadedDate = DateTime.UtcNow },
                new Tool { PartitionKey = "pk2", SKUMarker = "PWTL001", UnloadedDate = DateTime.UtcNow },
                new Tool { PartitionKey = "pk3", SKUMarker = "PWTL002", UnloadedDate = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            var result = await _repository.GetBySKUIdAsync("PWTL001");

            Assert.Equal(2, result.Count);
            Assert.All(result, t => Assert.Equal("PWTL001", t.SKUMarker));
        }

        [Fact]
        public async Task GetBySKUIdAsync_ReturnsEmptyList_WhenNotFound()
        {
            var result = await _repository.GetBySKUIdAsync("PWTL999");

            Assert.Empty(result);
        }

        [Fact]
        public async Task AddAsync_StagesItem_PersistedAfterSave()
        {
            var item = new Tool { PartitionKey = "pk1", SKUMarker = "PWTL001", UnloadedDate = DateTime.UtcNow };

            await _repository.AddAsync(item);
            await _context.SaveChangesAsync();

            Assert.Equal(1, await _context.Tools.CountAsync());
        }

        [Fact]
        public async Task UpdateBySKUIdAsync_UpdatesByPartitionKey_WhenMatch()
        {
            var original = new Tool { PartitionKey = "pk1", SKUMarker = "PWTL001", UnloadedDate = DateTime.UtcNow };
            var other = new Tool { PartitionKey = "pk2", SKUMarker = "PWTL001", UnloadedDate = DateTime.UtcNow };
            _context.Tools.AddRange(original, other);
            await _context.SaveChangesAsync();

            var updated = new Tool { PartitionKey = "pk1", SKUMarker = "PWTL001", UnloadedDate = DateTime.UtcNow.AddDays(1) };
            await _repository.UpdateBySKUIdAsync("PWTL001", updated);
            await _context.SaveChangesAsync();

            var result = await _context.Tools.FindAsync("pk1");
            Assert.Equal(updated.UnloadedDate, result!.UnloadedDate);
        }

        [Fact]
        public async Task UpdateBySKUIdAsync_FallsBackToFirst_WhenNoPartitionKeyMatch()
        {
            var item = new Tool { PartitionKey = "pk1", SKUMarker = "PWTL001", UnloadedDate = DateTime.UtcNow };
            _context.Tools.Add(item);
            await _context.SaveChangesAsync();

            var updated = new Tool { PartitionKey = "pk-nomatch", SKUMarker = "PWTL001", UnloadedDate = DateTime.UtcNow.AddDays(1) };
            await _repository.UpdateBySKUIdAsync("PWTL001", updated);
            await _context.SaveChangesAsync();

            var result = await _context.Tools.FindAsync("pk1");
            Assert.Equal(updated.UnloadedDate, result!.UnloadedDate);
        }

        [Fact]
        public async Task UpdateBySKUIdAsync_DoesNothing_WhenSkuNotFound()
        {
            var item = new Tool { PartitionKey = "pk1", SKUMarker = "PWTL001", UnloadedDate = DateTime.UtcNow };
            _context.Tools.Add(item);
            await _context.SaveChangesAsync();

            await _repository.UpdateBySKUIdAsync("PWTL999", new Tool { PartitionKey = "pk1", SKUMarker = "PWTL999", UnloadedDate = DateTime.UtcNow.AddDays(1) });
            await _context.SaveChangesAsync();

            var result = await _context.Tools.FindAsync("pk1");
            Assert.Equal(item.UnloadedDate, result!.UnloadedDate);
        }

        [Fact]
        public async Task DeleteBySKUIdAsync_ReturnsFalse_WhenNotFound()
        {
            var result = await _repository.DeleteBySKUIdAsync("PWTL999");

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteBySKUIdAsync_ReturnsTrue_AndRemovesAllMatchingItems()
        {
            _context.Tools.AddRange(
                new Tool { PartitionKey = "pk1", SKUMarker = "PWTL001", UnloadedDate = DateTime.UtcNow },
                new Tool { PartitionKey = "pk2", SKUMarker = "PWTL001", UnloadedDate = DateTime.UtcNow },
                new Tool { PartitionKey = "pk3", SKUMarker = "PWTL002", UnloadedDate = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            var result = await _repository.DeleteBySKUIdAsync("PWTL001");
            await _context.SaveChangesAsync();

            Assert.True(result);
            Assert.Equal(1, await _context.Tools.CountAsync());
        }
    }
}
