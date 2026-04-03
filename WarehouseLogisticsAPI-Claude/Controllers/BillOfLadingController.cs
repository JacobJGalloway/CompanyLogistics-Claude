using Microsoft.AspNetCore.Mvc;
using WarehouseLogistics_Claude.Controllers.Interfaces;
using WarehouseLogistics_Claude.Models;

namespace WarehouseLogistics_Claude.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillOfLadingController : ControllerBase, IBillOfLadingController
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BillOfLading billOfLading)
        {
            billOfLading.TransactionId = Guid.NewGuid().ToString()[..8].Replace("-", "");
            billOfLading.Status = "Created";
            billOfLading.PartitionKey = $"{billOfLading.TransactionId}-{Guid.NewGuid().ToString().Replace("-", "")}";

            const int lineWidth = 60;
            string transactionLine = $"Transaction ID: {billOfLading.TransactionId}";
            string centeredTransactionLine = transactionLine.PadLeft((lineWidth + transactionLine.Length) / 2).PadRight(lineWidth);

            int colWarehouse = Math.Max("Warehouse".Length,  billOfLading.LineEntries.DefaultIfEmpty().Max(e => e?.WarehouseId.Length ?? 0));
            int colSKU       = Math.Max("SKU Marker".Length, billOfLading.LineEntries.DefaultIfEmpty().Max(e => e?.SKUMarker.Length  ?? 0));
            int colQty       = Math.Max("Quantity".Length,   billOfLading.LineEntries.DefaultIfEmpty().Max(e => e?.Quantity.ToString().Length ?? 0));

            string tableTop     = $"┌{new string('─', colWarehouse + 2)}┬{new string('─', colSKU + 2)}┬{new string('─', colQty + 2)}┐";
            string tableHeader  = $"│ {"Warehouse".PadRight(colWarehouse)} │ {"SKU Marker".PadRight(colSKU)} │ {"Quantity".PadRight(colQty)} │";
            string tableDivider = $"├{new string('─', colWarehouse + 2)}┼{new string('─', colSKU + 2)}┼{new string('─', colQty + 2)}┤";
            string tableBottom  = $"└{new string('─', colWarehouse + 2)}┴{new string('─', colSKU + 2)}┴{new string('─', colQty + 2)}┘";

            var tableRows = billOfLading.LineEntries
                .Select(e => $"│ {e.WarehouseId.PadRight(colWarehouse)} │ {e.SKUMarker.PadRight(colSKU)} │ {e.Quantity.ToString().PadRight(colQty)} │");

            var fileLines = new[]
            {
                centeredTransactionLine,
                $"Client: {billOfLading.CustomerFirstName} {billOfLading.CustomerLastName}",
                $"City: {billOfLading.City} State: {billOfLading.State}",
                string.Empty,
                new string('─', lineWidth),
                Environment.GetEnvironmentVariable("LINE_ENTRIES_WORDING") ?? "Line Entries",
                new string('─', lineWidth),
                string.Empty,
                tableTop,
                tableHeader,
                tableDivider
            }
            .Concat(tableRows)
            .Append(tableBottom);

            string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            await System.IO.File.WriteAllLinesAsync(Path.Combine(downloadsPath, $"BillOfLading_{billOfLading.TransactionId}.txt"), fileLines);

            return CreatedAtAction(nameof(Create), new { transactionId = billOfLading.TransactionId }, billOfLading.TransactionId);
        }

        [HttpPost("{transactionId}/process")]
        public async Task<IActionResult> Process(string transactionId)
        {
            throw new NotImplementedException();
        }
    }
}
