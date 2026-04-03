using Microsoft.AspNetCore.Mvc;
using WarehouseInventory_Claude.Data.Interfaces;
using WarehouseInventory_Claude.Models;

namespace WarehouseInventory_Claude.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClothingController(IClothingRepository repository) : ControllerBase
    {
        private readonly IClothingRepository _repository = repository;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Clothing>>> GetAll()
        {
            return Ok(await _repository.GetAllAsync());
        }

        [HttpGet("{skuId}")]
        public async Task<ActionResult<Clothing>> GetBySKUId(string skuId)
        {
            var item = await _repository.GetBySKUIdAsync(skuId);
            if (item is null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<Clothing>> Create(Clothing item)
        {
            var created = await _repository.AddAsync(item);
            return CreatedAtAction(nameof(Create), new { id = created.PartitionKey }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBySKUId(string skuId, Clothing item)
        {
            if (skuId != item.SKUMarker) return BadRequest();
            await _repository.UpdateBySKUIdAsync(skuId, item);
            return NoContent();
        }

        [HttpDelete("{skuId}")]
        public async Task<IActionResult> DeleteBySKUIdAsync(string skuId)
        {
            if (!await _repository.DeleteBySKUIdAsync(skuId)) return NotFound();
            return NoContent();
        }
    }
}

