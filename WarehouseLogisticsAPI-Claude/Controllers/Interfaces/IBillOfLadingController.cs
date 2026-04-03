using Microsoft.AspNetCore.Mvc;
using WarehouseLogistics_Claude.Models;

namespace WarehouseLogistics_Claude.Controllers.Interfaces
{
    public interface IBillOfLadingController
    {
        Task<IActionResult> Create(BillOfLading billOfLading);
        Task<IActionResult> Process(string transactionId);
    }
}
