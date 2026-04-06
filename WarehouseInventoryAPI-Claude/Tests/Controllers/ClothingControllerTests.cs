using Microsoft.AspNetCore.Mvc;
using Moq;
using WarehouseInventory_Claude.Controllers;
using WarehouseInventory_Claude.Data.Interfaces;
using WarehouseInventory_Claude.Models;

namespace WarehouseInventory_Claude.Tests.Controllers
{
    public class ClothingControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUoW;
        private readonly Mock<IClothingRepository> _mockRepo;
        private readonly ClothingController _controller;

        public ClothingControllerTests()
        {
            _mockUoW = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<IClothingRepository>();
            _mockUoW.Setup(u => u.Clothing).Returns(_mockRepo.Object);
            _controller = new ClothingController(_mockUoW.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithItems()
        {
            var items = new List<Clothing>
            {
                new() { PartitionKey = "pk1", SKUMarker = "CLTH001", UnloadedDate = DateTime.UtcNow },
                new() { PartitionKey = "pk2", SKUMarker = "CLTH002", UnloadedDate = DateTime.UtcNow }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(items);

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(items, ok.Value);
        }

        [Fact]
        public async Task GetClothingBySKUId_ReturnsOkWithItems_WhenFound()
        {
            var items = new List<Clothing>
            {
                new() { PartitionKey = "pk1", SKUMarker = "CLTH001", UnloadedDate = DateTime.UtcNow }
            };
            _mockUoW.Setup(u => u.GetClothingBySKUIdAsync("CLTH001")).ReturnsAsync(items);

            var result = await _controller.GetClothingBySKUIdAsync("CLTH001");

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(items, ok.Value);
        }

        [Fact]
        public async Task GetClothingBySKUId_ReturnsEmptyList_WhenNotFound()
        {
            _mockUoW.Setup(u => u.GetClothingBySKUIdAsync("CLTH999")).ReturnsAsync(new List<Clothing>());

            var result = await _controller.GetClothingBySKUIdAsync("CLTH999");

            var list = Assert.IsType<List<Clothing>>(result.Value);
            Assert.Empty(list);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_AndSavesChanges()
        {
            var item = new Clothing { PartitionKey = "pk1", SKUMarker = "CLTH001", UnloadedDate = DateTime.UtcNow };
            _mockRepo.Setup(r => r.AddAsync(item)).ReturnsAsync(item);

            var result = await _controller.Create(item);

            Assert.IsType<CreatedAtActionResult>(result.Result);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateBySKUId_ReturnsBadRequest_WhenSkuMismatch()
        {
            var item = new Clothing { PartitionKey = "pk1", SKUMarker = "CLTH002", UnloadedDate = DateTime.UtcNow };

            var result = await _controller.UpdateBySKUId("CLTH001", item);

            Assert.IsType<BadRequestResult>(result);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateBySKUId_ReturnsNoContent_AndSavesChanges_WhenValid()
        {
            var item = new Clothing { PartitionKey = "pk1", SKUMarker = "CLTH001", UnloadedDate = DateTime.UtcNow };
            _mockRepo.Setup(r => r.UpdateBySKUIdAsync("CLTH001", item)).Returns(Task.CompletedTask);

            var result = await _controller.UpdateBySKUId("CLTH001", item);

            Assert.IsType<NoContentResult>(result);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteBySKUId_ReturnsNotFound_WhenNotFound()
        {
            _mockRepo.Setup(r => r.DeleteBySKUIdAsync("CLTH999")).ReturnsAsync(false);

            var result = await _controller.DeleteBySKUIdAsync("CLTH999");

            Assert.IsType<NotFoundResult>(result);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteBySKUId_ReturnsNoContent_AndSavesChanges_WhenFound()
        {
            _mockRepo.Setup(r => r.DeleteBySKUIdAsync("CLTH001")).ReturnsAsync(true);

            var result = await _controller.DeleteBySKUIdAsync("CLTH001");

            Assert.IsType<NoContentResult>(result);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
