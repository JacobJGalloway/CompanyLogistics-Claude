using Microsoft.AspNetCore.Mvc;
using WarehouseInventory_Claude.Data.Interfaces;
using WarehouseInventory_Claude.Models;

namespace WarehouseInventory_Claude.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PPEController(IUnitOfWork unitOfWork) : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PPE>>> GetAll()
        {
            return Ok(await _unitOfWork.PPE.GetAllAsync());
        }

        [HttpGet("{skuId}")]
        public async Task<ActionResult<List<PPE>>> GetBySKUId(string skuId)
        {
            var items = await _unitOfWork.GetPPEBySKUIdAsync(skuId);
            if (items.Count == 0) return new List<PPE>();
            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult<PPE>> Create(PPE item)
        {
            var created = await _unitOfWork.PPE.AddAsync(item);
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction(nameof(Create), new { id = created.PartitionKey }, created);
        }

        [HttpPut("{skuId}")]
        public async Task<IActionResult> UpdateBySKUId(string skuId, PPE item)
        {
            if (skuId != item.SKUMarker) return BadRequest();
            await _unitOfWork.PPE.UpdateBySKUIdAsync(skuId, item);
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{skuId}")]
        public async Task<IActionResult> DeleteBySKUId(string skuId)
        {
            if (!await _unitOfWork.PPE.DeleteBySKUIdAsync(skuId)) return NotFound();
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }
    }
}
