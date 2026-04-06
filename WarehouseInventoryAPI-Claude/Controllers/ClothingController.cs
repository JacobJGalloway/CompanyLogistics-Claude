using Microsoft.AspNetCore.Mvc;
using WarehouseInventory_Claude.Data.Interfaces;
using WarehouseInventory_Claude.Models;

namespace WarehouseInventory_Claude.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClothingController(IUnitOfWork unitOfWork) : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Clothing>>> GetAll()
        {
            return Ok(await _unitOfWork.Clothing.GetAllAsync());
        }

        [HttpGet("{skuId}")]
        public async Task<ActionResult<List<Clothing>>> GetClothingBySKUIdAsync(string skuId)
        {
            var items = await _unitOfWork.GetClothingBySKUIdAsync(skuId);
            if (items.Count == 0) return new List<Clothing>();
            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult<Clothing>> Create(Clothing item)
        {
            var created = await _unitOfWork.Clothing.AddAsync(item);
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction(nameof(Create), new { id = created.PartitionKey }, created);
        }

        [HttpPut("{skuId}")]
        public async Task<IActionResult> UpdateBySKUId(string skuId, Clothing item)
        {
            if (skuId != item.SKUMarker) return BadRequest();
            await _unitOfWork.Clothing.UpdateBySKUIdAsync(skuId, item);
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{skuId}")]
        public async Task<IActionResult> DeleteBySKUIdAsync(string skuId)
        {
            if (!await _unitOfWork.Clothing.DeleteBySKUIdAsync(skuId)) return NotFound();
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }
    }
}
