using Microsoft.AspNetCore.Mvc;
using Moq;
using WarehouseInventory_Claude.Controllers;
using WarehouseInventory_Claude.Data.Interfaces;
using WarehouseInventory_Claude.Models;

namespace WarehouseInventory_Claude.Tests.Controllers
{
    public class ToolControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUoW;
        private readonly Mock<IToolRepository> _mockRepo;
        private readonly ToolController _controller;

        public ToolControllerTests()
        {
            _mockUoW = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<IToolRepository>();
            _mockUoW.Setup(u => u.Tools).Returns(_mockRepo.Object);
            _controller = new ToolController(_mockUoW.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithItems()
        {
            var items = new List<Tool>
            {
                new() { PartitionKey = "pk1", SKUMarker = "PWTL001", UnloadedDate = DateTime.UtcNow },
                new() { PartitionKey = "pk2", SKUMarker = "PWTL002", UnloadedDate = DateTime.UtcNow }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(items);

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(items, ok.Value);
        }

        [Fact]
        public async Task GetBySKUId_ReturnsOkWithItems_WhenFound()
        {
            var items = new List<Tool>
            {
                new() { PartitionKey = "pk1", SKUMarker = "PWTL001", UnloadedDate = DateTime.UtcNow }
            };
            _mockUoW.Setup(u => u.GetToolBySKUIdAsync("PWTL001")).ReturnsAsync(items);

            var result = await _controller.GetBySKUIdAsync("PWTL001");

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(items, ok.Value);
        }

        [Fact]
        public async Task GetBySKUId_ReturnsEmptyList_WhenNotFound()
        {
            _mockUoW.Setup(u => u.GetToolBySKUIdAsync("PWTL999")).ReturnsAsync(new List<Tool>());

            var result = await _controller.GetBySKUIdAsync("PWTL999");

            var list = Assert.IsType<List<Tool>>(result.Value);
            Assert.Empty(list);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_AndSavesChanges()
        {
            var item = new Tool { PartitionKey = "pk1", SKUMarker = "PWTL001", UnloadedDate = DateTime.UtcNow };
            _mockRepo.Setup(r => r.AddAsync(item)).ReturnsAsync(item);

            var result = await _controller.Create(item);

            Assert.IsType<CreatedAtActionResult>(result.Result);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateBySKUId_ReturnsBadRequest_WhenSkuMismatch()
        {
            var item = new Tool { PartitionKey = "pk1", SKUMarker = "PWTL002", UnloadedDate = DateTime.UtcNow };

            var result = await _controller.UpdateBySKUId("PWTL001", item);

            Assert.IsType<BadRequestResult>(result);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateBySKUId_ReturnsNoContent_AndSavesChanges_WhenValid()
        {
            var item = new Tool { PartitionKey = "pk1", SKUMarker = "PWTL001", UnloadedDate = DateTime.UtcNow };
            _mockRepo.Setup(r => r.UpdateBySKUIdAsync("PWTL001", item)).Returns(Task.CompletedTask);

            var result = await _controller.UpdateBySKUId("PWTL001", item);

            Assert.IsType<NoContentResult>(result);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteBySKUId_ReturnsNotFound_WhenNotFound()
        {
            _mockRepo.Setup(r => r.DeleteBySKUIdAsync("PWTL999")).ReturnsAsync(false);

            var result = await _controller.DeleteBySKUId("PWTL999");

            Assert.IsType<NotFoundResult>(result);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteBySKUId_ReturnsNoContent_AndSavesChanges_WhenFound()
        {
            _mockRepo.Setup(r => r.DeleteBySKUIdAsync("PWTL001")).ReturnsAsync(true);

            var result = await _controller.DeleteBySKUId("PWTL001");

            Assert.IsType<NoContentResult>(result);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
