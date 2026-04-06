using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WarehouseLogistics_Claude.Models;

namespace WarehouseLogistics_Claude.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillOfLadingController : ControllerBase
    {
        // pull the originating warehouse from the environment variable and ensure it is the first line entry if provided. 
        // This allows for more efficient routing and load planning, as the driver can determine the remaining stops based on distance and inventory levels.
        static readonly string locationId = Environment.GetEnvironmentVariable("LOCATION_ID") ?? string.Empty;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BillOfLading billOfLading)
        {               
            if (billOfLading.LineEntries == null || !billOfLading.LineEntries.Any() || 
                string.IsNullOrWhiteSpace(locationId))
            {
                return BadRequest($"The first line entry must have the originating warehouse ID: {locationId}.");
            }
            
            if (billOfLading == null || billOfLading.Status != "Pending")
            {
                return BadRequest("a Pending Bill of Lading is required.");
            }

            if (string.IsNullOrWhiteSpace(billOfLading.CustomerFirstName) ||
                string.IsNullOrWhiteSpace(billOfLading.CustomerLastName) ||
                string.IsNullOrWhiteSpace(billOfLading.City) ||
                string.IsNullOrWhiteSpace(billOfLading.State))
            {
                return BadRequest("Customer information is required.");
            }

            if (billOfLading.LineEntries == null || !billOfLading.LineEntries.Any())
            {
                return BadRequest("At least one line entry is required.");
            }

            billOfLading.TransactionId = Guid.NewGuid().ToString()[..8].Replace("-", "");
            billOfLading.Status = "Submitted";
            billOfLading.PartitionKey = $"{billOfLading.TransactionId}-{Guid.NewGuid().ToString().Replace("-", "")}";

            const int lineWidth = 60;

            string warehouseIdLine, centeredWarehouseIdLine = string.Empty;

            if (!string.IsNullOrWhiteSpace(locationId))
            {
                warehouseIdLine = $"Warehouse ID: {locationId}";
                centeredWarehouseIdLine = locationId.Substring(0,2) == "WH" ?
                  warehouseIdLine.PadLeft((lineWidth + warehouseIdLine.Length) / 2).PadRight(lineWidth)
                  : string.Empty;
            }
            
            string transactionLine = $"Transaction ID: {billOfLading.TransactionId}";
            string centeredTransactionLine = transactionLine.PadLeft((lineWidth + transactionLine.Length) / 2).PadRight(lineWidth);

            int colWarehouse = Math.Max("Warehouse".Length,  billOfLading.LineEntries.DefaultIfEmpty().Max(e => e?.LocationId.Length ?? 0));
            int colSKU       = Math.Max("SKU Marker".Length, billOfLading.LineEntries.DefaultIfEmpty().Max(e => e?.SKUMarker.Length  ?? 0));
            int colQty       = Math.Max("Quantity".Length,   billOfLading.LineEntries.DefaultIfEmpty().Max(e => e?.Quantity.ToString().Length ?? 0));

            string tableTop     = $"┌{new string('─', colWarehouse + 2)}┬{new string('─', colSKU + 2)}┬{new string('─', colQty + 2)}┐";
            string tableHeader  = $"│ {"Warehouse".PadRight(colWarehouse)} │ {"SKU Marker".PadRight(colSKU)} │ {"Quantity".PadRight(colQty)} │";
            string tableDivider = $"├{new string('─', colWarehouse + 2)}┼{new string('─', colSKU + 2)}┼{new string('─', colQty + 2)}┤";
            string tableBottom  = $"└{new string('─', colWarehouse + 2)}┴{new string('─', colSKU + 2)}┴{new string('─', colQty + 2)}┘";

            List<string> tableRows = [];

            if (string.IsNullOrWhiteSpace(locationId))
            {
                tableRows = billOfLading.LineEntries
                    .OrderBy(e => e.LocationId)
                    .Select(e => $"│ {e.LocationId.PadRight(colWarehouse)} │ {e.SKUMarker.PadRight(colSKU)} │ {e.Quantity.ToString().PadRight(colQty)} │")
                    .ToList<string>();
            }
            else
            {
                tableRows = billOfLading.LineEntries
                    .Where(e => string.Equals(e.LocationId, locationId))
                    .Select(e => $"│ {e.LocationId.PadRight(colWarehouse)} │ {e.SKUMarker.PadRight(colSKU)} │ {e.Quantity.ToString().PadRight(colQty)} │")
                    .ToList<string>();                   
            
                tableRows.AddRange(billOfLading.LineEntries
                    .Where(e => !string.Equals(e.LocationId, locationId))
                    .OrderBy(e => e.LocationId)
                    .Select(e => $"│ {e.LocationId.PadRight(colWarehouse)} │ {e.SKUMarker.PadRight(colSKU)} │ {e.Quantity.ToString().PadRight(colQty)} │")
                    
                    .ToList<string>());
            }

            var fileLines = new[]
            {
                centeredWarehouseIdLine,
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

            // First warehouse is entered and then the driver can determine the remaining stops based on distance and inventory levels. This 
            // allows for more efficient routing and load planning.
            await ProcessLocationStop(transactionId: billOfLading.TransactionId, locationId: locationId);

            return CreatedAtAction(nameof(Create), new { transactionId = billOfLading.TransactionId }, billOfLading.TransactionId);
        }

        [HttpPost("{transactionId}/process/{locationId}")]
        public async Task<IActionResult> ProcessLocationStop(string transactionId, string locationId)
        {
            throw new NotImplementedException();
        }
    }
}
