using Microsoft.AspNetCore.Mvc;
using Moq;
using WarehouseInventory_Claude.Controllers;
using WarehouseInventory_Claude.Data.Interfaces;
using WarehouseInventory_Claude.Models;

namespace WarehouseInventory_Claude.Tests.Controllers
{
    public class PPEControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUoW;
        private readonly Mock<IPPERepository> _mockRepo;
        private readonly PPEController _controller;

        public PPEControllerTests()
        {
            _mockUoW = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<IPPERepository>();
            _mockUoW.Setup(u => u.PPE).Returns(_mockRepo.Object);
            _controller = new PPEController(_mockUoW.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithItems()
        {
            var items = new List<PPE>
            {
                new() { PartitionKey = "pk1", SKUMarker = "SPPE001", UnloadedDate = DateTime.UtcNow },
                new() { PartitionKey = "pk2", SKUMarker = "SPPE002", UnloadedDate = DateTime.UtcNow }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(items);

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(items, ok.Value);
        }

        [Fact]
        public async Task GetBySKUId_ReturnsOkWithItems_WhenFound()
        {
            var items = new List<PPE>
            {
                new() { PartitionKey = "pk1", SKUMarker = "SPPE001", UnloadedDate = DateTime.UtcNow }
            };
            _mockUoW.Setup(u => u.GetPPEBySKUIdAsync("SPPE001")).ReturnsAsync(items);

            var result = await _controller.GetBySKUId("SPPE001");

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(items, ok.Value);
        }

        [Fact]
        public async Task GetBySKUId_ReturnsEmptyList_WhenNotFound()
        {
            _mockUoW.Setup(u => u.GetPPEBySKUIdAsync("SPPE999")).ReturnsAsync(new List<PPE>());

            var result = await _controller.GetBySKUId("SPPE999");

            var list = Assert.IsType<List<PPE>>(result.Value);
            Assert.Empty(list);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_AndSavesChanges()
        {
            var item = new PPE { PartitionKey = "pk1", SKUMarker = "SPPE001", UnloadedDate = DateTime.UtcNow };
            _mockRepo.Setup(r => r.AddAsync(item)).ReturnsAsync(item);

            var result = await _controller.Create(item);

            Assert.IsType<CreatedAtActionResult>(result.Result);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateBySKUId_ReturnsBadRequest_WhenSkuMismatch()
        {
            var item = new PPE { PartitionKey = "pk1", SKUMarker = "SPPE002", UnloadedDate = DateTime.UtcNow };

            var result = await _controller.UpdateBySKUId("SPPE001", item);

            Assert.IsType<BadRequestResult>(result);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateBySKUId_ReturnsNoContent_AndSavesChanges_WhenValid()
        {
            var item = new PPE { PartitionKey = "pk1", SKUMarker = "SPPE001", UnloadedDate = DateTime.UtcNow };
            _mockRepo.Setup(r => r.UpdateBySKUIdAsync("SPPE001", item)).Returns(Task.CompletedTask);

            var result = await _controller.UpdateBySKUId("SPPE001", item);

            Assert.IsType<NoContentResult>(result);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteBySKUId_ReturnsNotFound_WhenNotFound()
        {
            _mockRepo.Setup(r => r.DeleteBySKUIdAsync("SPPE999")).ReturnsAsync(false);

            var result = await _controller.DeleteBySKUId("SPPE999");

            Assert.IsType<NotFoundResult>(result);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteBySKUId_ReturnsNoContent_AndSavesChanges_WhenFound()
        {
            _mockRepo.Setup(r => r.DeleteBySKUIdAsync("SPPE001")).ReturnsAsync(true);

            var result = await _controller.DeleteBySKUId("SPPE001");

            Assert.IsType<NoContentResult>(result);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
