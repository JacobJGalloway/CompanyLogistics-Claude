using Microsoft.AspNetCore.Mvc;
using WarehouseInventory_Claude.Data.Interfaces;
using WarehouseInventory_Claude.Models;

namespace WarehouseInventory_Claude.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToolController(IUnitOfWork unitOfWork) : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tool>>> GetAll()
        {
            return Ok(await _unitOfWork.Tools.GetAllAsync());
        }

        [HttpGet("{skuId}")]
        public async Task<ActionResult<List<Tool>>> GetBySKUIdAsync(string skuId)
        {
            var items = await _unitOfWork.GetToolBySKUIdAsync(skuId);
            if (items.Count == 0) return new List<Tool>();
            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult<Tool>> Create(Tool item)
        {
            var created = await _unitOfWork.Tools.AddAsync(item);
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction(nameof(Create), new { skuId = created.PartitionKey }, created);
        }

        [HttpPut("{skuId}")]
        public async Task<IActionResult> UpdateBySKUId(string skuId, Tool item)
        {
            if (skuId != item.SKUMarker) return BadRequest();
            await _unitOfWork.Tools.UpdateBySKUIdAsync(skuId, item);
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{skuId}")]
        public async Task<IActionResult> DeleteBySKUId(string skuId)
        {
            if (!await _unitOfWork.Tools.DeleteBySKUIdAsync(skuId)) return NotFound();
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }
    }
}
