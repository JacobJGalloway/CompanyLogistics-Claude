using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseInventory_Claude.Data.Interfaces;
using WarehouseInventory_Claude.Models;

namespace WarehouseInventory_Claude.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PPEController(IPPERepository repository) : ControllerBase
    {
        private readonly IPPERepository _repository = repository;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PPE>>> GetAll()
        {
            return Ok(await _repository.GetAllAsync());
        }

        [HttpGet("{skuId}")]
        public async Task<ActionResult<PPE>> GetBySKUId(string skuId)
        {
            var item = await _repository.GetBySKUIdAsync(skuId);
            if (item is null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<PPE>> Create(PPE item)
        {
            var created = await _repository.AddAsync(item);
            return CreatedAtAction(nameof(Create), new { id = created.PartitionKey }, created);
        }

        [HttpPut("{skuId}")]
        public async Task<IActionResult> UpdateBySKUId(string skuId, PPE item)
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

