using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseInventory_Claude.Data.Interfaces;
using WarehouseInventory_Claude.Models;

namespace WarehouseInventory_Claude.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToolController(IToolRepository repository) : ControllerBase
    {
        private readonly IToolRepository _repository = repository;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tool>>> GetAll()
        {
            return Ok(await _repository.GetAllAsync());
        }

        [HttpGet("{skuId}")]
        public async Task<ActionResult<Tool>> GetBySKUId(string skuId)
        {
            var item = await _repository.GetBySKUIdAsync(skuId);
            if (item is null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<Tool>> Create(Tool item)
        {
            var created = await _repository.AddAsync(item);
            return CreatedAtAction(nameof(Create), new { skuId = created.PartitionKey }, created);
        }

        [HttpPut("{skuId}")]
        public async Task<IActionResult> UpdateBySKUId(string skuId, Tool item)
        {
            if (skuId != item.SKUMarker) return BadRequest();
            await _repository.UpdateBySKUIdAsync(skuId, item);
            return NoContent();
        }

        [HttpDelete("{skuId}")]
        public async Task<IActionResult> DeleteBySKUId(string skuId)
        {
            if (!await _repository.DeleteBySKUIdAsync(skuId)) return NotFound();
            return NoContent();
        }
    }
}

